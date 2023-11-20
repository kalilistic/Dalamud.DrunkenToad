namespace Dalamud.DrunkenToad.Core.Services;

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using Core;
using Hooking;
using Memory;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Models;

/// <summary>
/// Provides events when data is received for social lists (e.g. Friends List).
/// </summary>
public unsafe class SocialListHandler
{
    private const int VtblFuncIndex = 6;
    private const int BlackListStringArray = 14;
    private const int BlackListStartIndex = 200;

    private Hook<InfoProxyFriendListEndRequestDelegate>? infoProxyFriendListEndRequestHook;

    private Hook<InfoProxyFreeCompanyEndRequestDelegate>? infoProxyFreeCompanyEndRequestHook;

    private Hook<InfoProxyLinkShellEndRequestDelegate>? infoProxyLinkShellEndRequestHook;

    private Hook<InfoProxyCrossWorldLinkShellEndRequestDelegate>? infoProxyCrossWorldLinkShellEndRequestHook;

    private Hook<InfoProxyBlackListEndRequestDelegate>? infoProxyBlackListEndRequestHook;

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

    private delegate void InfoProxyFriendListEndRequestDelegate(InfoProxyInterface* a1);

    private delegate void InfoProxyFreeCompanyEndRequestDelegate(InfoProxyInterface* a1);

    private delegate void InfoProxyLinkShellEndRequestDelegate(InfoProxyInterface* a1);

    private delegate void InfoProxyCrossWorldLinkShellEndRequestDelegate(InfoProxyInterface* a1);

    private delegate void InfoProxyBlackListEndRequestDelegate(InfoProxyInterface* a1);

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
                Name = MemoryHelper.ReadSeStringNullTerminated((nint)entry->Name).ToString(),
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
        for (uint i = 0; i < infoProxyInterface->EntryCount; i++)
        {
            var member = new ToadSocialListMember
            {
                ContentId = (ulong)((InfoProxyBlacklist*)infoProxyInterface)->ContentIdArray[i],
            };

            var data = (nint*)AtkStage.GetSingleton()->AtkArrayDataHolder->StringArrays[BlackListStringArray]->StringArray;
            var worldName = MemoryHelper.ReadStringNullTerminated(data[200 + i]);
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
        var infoProxyFriendListEndRequestAddress = (nint)Framework.Instance()->GetUiModule()->GetInfoModule()->GetInfoProxyById(InfoProxyId.FriendList)->vtbl[VtblFuncIndex];
        this.infoProxyFriendListEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyFriendListEndRequestDelegate>(infoProxyFriendListEndRequestAddress, this.InfoProxyFriendListEndRequestDetour);
        this.infoProxyFriendListEndRequestHook.Enable();
    }

    private void SetupFreeCompany()
    {
        var infoProxyFreeCompanyEndRequestAddress = (nint)Framework.Instance()->GetUiModule()->GetInfoModule()->GetInfoProxyById(InfoProxyId.FreeCompanyMember)->vtbl[VtblFuncIndex];
        this.infoProxyFreeCompanyEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyFreeCompanyEndRequestDelegate>(infoProxyFreeCompanyEndRequestAddress, this.InfoProxyFreeCompanyEndRequestDetour);
        this.infoProxyFreeCompanyEndRequestHook.Enable();
    }

    private void SetupLinkShell()
    {
        var infoProxyLinkShellEndRequestAddress = (nint)Framework.Instance()->GetUiModule()->GetInfoModule()->GetInfoProxyById(InfoProxyId.LinkShellMember)->vtbl[VtblFuncIndex];
        this.infoProxyLinkShellEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyLinkShellEndRequestDelegate>(infoProxyLinkShellEndRequestAddress, this.InfoProxyLinkShellEndRequestDetour);
        this.infoProxyLinkShellEndRequestHook.Enable();
    }

    private void SetupCrossWorldLinkShell()
    {
        var infoProxyCrossWorldLinkShellEndRequestAddress = (nint)Framework.Instance()->GetUiModule()->GetInfoModule()->GetInfoProxyById(InfoProxyId.CrossWorldLinkShellMember)->vtbl[VtblFuncIndex];
        this.infoProxyCrossWorldLinkShellEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyCrossWorldLinkShellEndRequestDelegate>(infoProxyCrossWorldLinkShellEndRequestAddress, this.InfoProxyCrossWorldLinkShellEndRequestDetour);
        this.infoProxyCrossWorldLinkShellEndRequestHook.Enable();
    }

    private void SetupBlackList()
    {
        var infoProxyBlackListEndRequestAddress = (nint)Framework.Instance()->GetUiModule()->GetInfoModule()->GetInfoProxyById(InfoProxyId.Blacklist)->vtbl[VtblFuncIndex];
        this.infoProxyBlackListEndRequestHook = DalamudContext.HookManager.HookFromAddress<InfoProxyBlackListEndRequestDelegate>(infoProxyBlackListEndRequestAddress, this.InfoProxyBlackListEndRequestDetour);
        this.infoProxyBlackListEndRequestHook.Enable();
    }

    private void InfoProxyFriendListEndRequestDetour(InfoProxyInterface* infoProxyInterface)
    {
        DalamudContext.PluginLog.Verbose("Entering InfoProxyFriendListEndRequestDetour");
        this.infoProxyFriendListEndRequestHook?.Original(infoProxyInterface);
        if (!this.isEnabled)
        {
            return;
        }

        try
        {
            this.FriendListReceived?.Invoke(ExtractInfoProxyMembers(infoProxyInterface));
        }
        catch (Exception ex)
        {
            DalamudContext.PluginLog.Error(ex, "Exception in InfoProxyFriendListEndRequestDetour");
        }
    }

    private void InfoProxyFreeCompanyEndRequestDetour(InfoProxyInterface* infoProxyInterface)
    {
        DalamudContext.PluginLog.Verbose("Entering InfoProxyFreeCompanyEndRequestDetour");
        this.infoProxyFreeCompanyEndRequestHook?.Original(infoProxyInterface);
        if (!this.isEnabled)
        {
            return;
        }

        try
        {
            var proxyFC = (InfoProxyFreeCompany*)Framework.Instance()->GetUiModule()->GetInfoModule()->GetInfoProxyById(InfoProxyId.FreeCompany);
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

            var agentFC = (nint)Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.FreeCompany);
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
        DalamudContext.PluginLog.Verbose("Entering InfoProxyCrossWorldLinkShellEndRequestDetour");
        this.infoProxyCrossWorldLinkShellEndRequestHook?.Original(infoProxyInterface);
        if (!this.isEnabled)
        {
            return;
        }

        try
        {
            var agentCrossWorldLinkShell = (AgentCrossWorldLinkshell*)Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.CrossWorldLinkShell);
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
        DalamudContext.PluginLog.Verbose("Entering InfoProxyLinkShellEndRequestDetour");
        this.infoProxyLinkShellEndRequestHook?.Original(infoProxyInterface);
        if (!this.isEnabled)
        {
            return;
        }

        try
        {
            var agentLinkShell = (AgentLinkshell*)Framework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Linkshell);
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
        DalamudContext.PluginLog.Verbose("Entering InfoProxyBlackListEndRequestDetour");
        this.infoProxyBlackListEndRequestHook?.Original(infoProxyInterface);
        if (!this.isEnabled)
        {
            return;
        }

        try
        {
            this.BlackListReceived?.Invoke(ExtractInfoProxyBlackListMembers(infoProxyInterface));
        }
        catch (Exception ex)
        {
            DalamudContext.PluginLog.Error(ex, "Exception in InfoProxyBlackListEndRequestDetour");
        }
    }
}
