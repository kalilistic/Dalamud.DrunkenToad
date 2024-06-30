// ReSharper disable All
#pragma warning disable CS0414 // Field is assigned but its value is never used
#pragma warning disable CS0067 // Event is never used
namespace Dalamud.DrunkenToad.Core.Services;

using System;
using System.Collections.Generic;
using Core;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Models;

/// <summary>
/// Provides events when data is received for social lists (e.g. Friends List).
/// </summary>
public unsafe class SocialListHandler
{
    private bool isEnabled;

    /// <summary>
    /// Initializes a new instance of the <see cref="SocialListHandler" /> class.
    /// </summary>
    public SocialListHandler()
    {
        DalamudContext.PluginLog.Verbose("Entering SocialListHandler.Start()");
        throw new NotImplementedException("Temporarily disabled due to Dawntrail changes.");
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

    public static void Dispose() => DalamudContext.PluginLog.Verbose("Entering SocialListHandler.Dispose()");

    public void Start() => this.isEnabled = true;
}
