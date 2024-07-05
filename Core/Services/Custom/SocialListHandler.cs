namespace Dalamud.DrunkenToad.Core.Services;

using System;
using System.Collections.Generic;
using System.Data;
using Core;
using Hooking;
using Memory;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Models;

/// <summary>
/// Provides events when data is received for social lists (e.g. Friends List).
/// </summary>
public unsafe class SocialListHandler
{
    private const int BlackListStringArray = 14;
    private const int BlackListWorldStartIndex = 200;

    private Hook<InfoProxyInterface.Delegates.EndRequest>? infoProxyFriendListEndRequestHook;

    private Hook<InfoProxyInterface.Delegates.EndRequest>? infoProxyFreeCompanyEndRequestHook;

    private Hook<InfoProxyInterface.Delegates.EndRequest>? infoProxyLinkShellEndRequestHook;

    private Hook<InfoProxyInterface.Delegates.EndRequest>? infoProxyCrossWorldLinkShellEndRequestHook;

    private Hook<InfoProxyInterface.Delegates.EndRequest>? infoProxyBlackListEndRequestHook;

    private bool isEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="SocialListHandler" /> class.
    /// </summary>
    public SocialListHandler()
    {
        DalamudContext.PluginLog.Verbose("Entering SocialListHandler.Start()");
        this.SetupFriendList();
        this.SetupFreeCompany();
        this.SetupLinkShell();
        this.SetupCrossWorldLinkShell();
        this.SetupBlackList();
    }

    public delegate void FriendListReceivedDelegate(List<ToadSocialListMember> members);

    public delegate void FreeCompanyReceivedDelegate(byte pageCount, byte currentPage, List<ToadSocialListMember> members);

    public delegate void LinkShellReceivedDelegate(byte index, List<ToadSocialListMember> members);

    public delegate void CrossWorldLinkShellReceivedDelegate(byte index, List<ToadSocialListMember> members);

    public delegate void BlackListReceivedDelegate(List<ToadSocialListMember> members);

    public event FriendListReceivedDelegate? FriendListReceived;

    public event FreeCompanyReceivedDelegate? FreeCompanyReceived;

    public event LinkShellReceivedDelegate? LinkShellReceived;

    public event CrossWorldLinkShellReceivedDelegate? CrossWorldLinkShellReceived;

    public event BlackListReceivedDelegate? BlackListReceived;

    public void Dispose()
    {
        DalamudContext.PluginLog.Verbose("Entering SocialListHandler.Dispose()");
        this.infoProxyFriendListEndRequestHook?.Dispose();
        this.infoProxyFreeCompanyEndRequestHook?.Dispose();
        this.infoProxyLinkShellEndRequestHook?.Dispose();
        this.infoProxyCrossWorldLinkShellEndRequestHook?.Dispose();
        this.infoProxyBlackListEndRequestHook?.Dispose();
    }

    public void Start() => this.isEnabled = true;

    private static List<ToadSocialListMember> ExtractInfoProxyMembers(InfoProxyInterface* infoProxyInterface)
    {
        DalamudContext.PluginLog.Verbose($"Entering ExtractInfoProxyMembers: EntryCount: {infoProxyInterface->EntryCount}");
        var members = new List<ToadSocialListMember>();
        for (uint i = 0; i < infoProxyInterface->EntryCount; i++)
        {
            var entry = ((InfoProxyCommonList*)infoProxyInterface)->GetEntry(i);
            var member = new ToadSocialListMember
            {
                ContentId = entry->ContentId,
                Name = entry->NameString,
                HomeWorld = entry->HomeWorld,
            };

            if (string.IsNullOrEmpty(member.Name))
            {
                member.IsUnableToRetrieve = true;
            }

            if (!member.IsValid())
            {
                throw new DataException($"Invalid member: {member.Name} {member.ContentId} {member.HomeWorld}");
            }

            members.Add(member);
        }

        return members;
    }

    private static List<ToadSocialListMember> ExtractInfoProxyBlackListMembers(InfoProxyInterface* infoProxyInterface)
    {
        DalamudContext.PluginLog.Verbose($"Entering ExtractInfoProxyBlackListMembers: EntryCount: {infoProxyInterface->EntryCount}");
        var members = new List<ToadSocialListMember>();
        for (var i = 0; i < infoProxyInterface->EntryCount; i++)
        {
            var member = new ToadSocialListMember
            {
                //ContentId = (ulong)((InfoProxyBlacklist*)infoProxyInterface)->ContentIds[i], // InfoProxyBlacklist changed
            };

            var data = (nint*)AtkStage.Instance()->AtkArrayDataHolder->StringArrays[BlackListStringArray]->StringArray;
            var worldName = MemoryHelper.ReadStringNullTerminated(data[BlackListWorldStartIndex + i]);
            if (!string.IsNullOrEmpty(worldName))
            {
                member.Name = MemoryHelper.ReadStringNullTerminated(data[i]);
                member.HomeWorld = (ushort)DalamudContext.DataManager.GetWorldIdByName(worldName);
            }
            else
            {
                member.Name = string.Empty;
                member.HomeWorld = 0;
                member.IsUnableToRetrieve = true;
            }

            if (!member.IsValid())
            {
                throw new DataException($"Invalid member: {member.Name} {member.ContentId} {member.HomeWorld}");
            }

            members.Add(member);
        }

        return members;
    }

    private void SetupFriendList()
    {
        var infoProxyFriendListEndRequestAddress = (nint)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.FriendList)->VirtualTable->EndRequest;
        this.infoProxyFriendListEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyInterface.Delegates.EndRequest>(infoProxyFriendListEndRequestAddress, this.InfoProxyFriendListEndRequestDetour);
        this.infoProxyFriendListEndRequestHook.Enable();
    }

    private void SetupFreeCompany()
    {
        var infoProxyFreeCompanyEndRequestAddress = (nint)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.FreeCompanyMember)->VirtualTable->EndRequest;
        this.infoProxyFreeCompanyEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyInterface.Delegates.EndRequest>(infoProxyFreeCompanyEndRequestAddress, this.InfoProxyFreeCompanyEndRequestDetour);
        this.infoProxyFreeCompanyEndRequestHook.Enable();
    }

    private void SetupLinkShell()
    {
        var infoProxyLinkShellEndRequestAddress = (nint)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.LinkshellMember)->VirtualTable->EndRequest;
        this.infoProxyLinkShellEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyInterface.Delegates.EndRequest>(infoProxyLinkShellEndRequestAddress, this.InfoProxyLinkShellEndRequestDetour);
        this.infoProxyLinkShellEndRequestHook.Enable();
    }

    private void SetupCrossWorldLinkShell()
    {
        var infoProxyCrossWorldLinkShellEndRequestAddress = (nint)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.CrossWorldLinkshellMember)->VirtualTable->EndRequest;
        this.infoProxyCrossWorldLinkShellEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyInterface.Delegates.EndRequest>(infoProxyCrossWorldLinkShellEndRequestAddress, this.InfoProxyCrossWorldLinkShellEndRequestDetour);
        this.infoProxyCrossWorldLinkShellEndRequestHook.Enable();
    }

    private void SetupBlackList()
    {
        var infoProxyBlackListEndRequestAddress = (nint)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.Blacklist)->VirtualTable->EndRequest;
        this.infoProxyBlackListEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyInterface.Delegates.EndRequest>(infoProxyBlackListEndRequestAddress, this.InfoProxyBlackListEndRequestDetour);
        this.infoProxyBlackListEndRequestHook.Enable();
    }

    private void InfoProxyFriendListEndRequestDetour(InfoProxyInterface* infoProxyInterface)
    {
        try
        {
            DalamudContext.PluginLog.Verbose("Entering InfoProxyFriendListEndRequestDetour");
            this.infoProxyFriendListEndRequestHook?.Original(infoProxyInterface);
            if (!this.isEnabled)
            {
                return;
            }

            this.FriendListReceived?.Invoke(ExtractInfoProxyMembers(infoProxyInterface));
        }
        catch (Exception ex)
        {
            DalamudContext.PluginLog.Error(ex, "Exception in InfoProxyFriendListEndRequestDetour");
        }
    }

    private void InfoProxyFreeCompanyEndRequestDetour(InfoProxyInterface* infoProxyInterface)
    {
        try
        {
            DalamudContext.PluginLog.Verbose("Entering InfoProxyFreeCompanyEndRequestDetour");
            this.infoProxyFreeCompanyEndRequestHook?.Original(infoProxyInterface);
            if (!this.isEnabled)
            {
                return;
            }

            var proxyFC = (InfoProxyFreeCompany*)InfoModule.Instance()->GetInfoProxyById(InfoProxyId.FreeCompany);
            if (proxyFC == null || proxyFC->TotalMembers == 0 || proxyFC->ActiveListItemNum != 1)
            {
                DalamudContext.PluginLog.Verbose("No FC members to process.");
                return;
            }

            var maxPage = (proxyFC->TotalMembers / 200) + 1;
            if (maxPage is < 1 or > 3)
            {
                DalamudContext.PluginLog.Warning($"Invalid FC page count: {maxPage}");
                return;
            }

            var agentFC = (nint)AgentModule.Instance()->GetAgentByInternalId(AgentId.FreeCompany);
            if (agentFC == nint.Zero)
            {
                DalamudContext.PluginLog.Warning("Failed to get FC agent.");
                return;
            }

            var pageIndex = *(byte*)(agentFC + 0x5E);
            var currentPage = pageIndex + 1;
            if (currentPage > maxPage)
            {
                DalamudContext.PluginLog.Warning($"Invalid FC page: {currentPage}");
                return;
            }

            var members = ExtractInfoProxyMembers(infoProxyInterface);
            this.FreeCompanyReceived?.Invoke((byte)maxPage, (byte)currentPage, members);
        }
        catch (Exception ex)
        {
            DalamudContext.PluginLog.Error(ex, "Exception in InfoProxyFreeCompanyEndRequestDetour");
        }
    }

    private void InfoProxyCrossWorldLinkShellEndRequestDetour(InfoProxyInterface* infoProxyInterface)
    {
        try
        {
            DalamudContext.PluginLog.Verbose("Entering InfoProxyCrossWorldLinkShellEndRequestDetour");
            this.infoProxyCrossWorldLinkShellEndRequestHook?.Original(infoProxyInterface);
            if (!this.isEnabled)
            {
                return;
            }

            var agentCrossWorldLinkShell = (AgentCrossWorldLinkshell*)AgentModule.Instance()->GetAgentByInternalId(AgentId.CrossWorldLinkShell);
            var index = agentCrossWorldLinkShell != null ? agentCrossWorldLinkShell->SelectedCWLSIndex : (byte)0;
            this.CrossWorldLinkShellReceived?.Invoke(index, ExtractInfoProxyMembers(infoProxyInterface));
        }
        catch (Exception ex)
        {
            DalamudContext.PluginLog.Error(ex, "Exception in InfoProxyCrossWorldLinkShellEndRequestDetour");
        }
    }

    private void InfoProxyLinkShellEndRequestDetour(InfoProxyInterface* infoProxyInterface)
    {
        try
        {
            DalamudContext.PluginLog.Verbose("Entering InfoProxyLinkShellEndRequestDetour");
            this.infoProxyLinkShellEndRequestHook?.Original(infoProxyInterface);
            if (!this.isEnabled)
            {
                return;
            }

            var agentLinkShell = (AgentLinkshell*)AgentModule.Instance()->GetAgentByInternalId(AgentId.Linkshell);
            var index = agentLinkShell != null ? agentLinkShell->SelectedLSIndex : (byte)0;
            this.LinkShellReceived?.Invoke(index, ExtractInfoProxyMembers(infoProxyInterface));
        }
        catch (Exception ex)
        {
            DalamudContext.PluginLog.Error(ex, "Exception in InfoProxyLinkShellEndRequestDetour");
        }
    }

    private void InfoProxyBlackListEndRequestDetour(InfoProxyInterface* infoProxyInterface)
    {
        try
        {
            DalamudContext.PluginLog.Verbose("Entering InfoProxyBlackListEndRequestDetour");
            this.infoProxyBlackListEndRequestHook?.Original(infoProxyInterface);
            if (!this.isEnabled)
            {
                return;
            }

            this.BlackListReceived?.Invoke(ExtractInfoProxyBlackListMembers(infoProxyInterface));
        }
        catch (Exception ex)
        {
            DalamudContext.PluginLog.Error(ex, "Exception in InfoProxyBlackListEndRequestDetour");
        }
    }
}
