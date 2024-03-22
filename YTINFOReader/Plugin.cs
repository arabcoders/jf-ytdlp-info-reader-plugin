using System;
using YTINFOReader.Helpers;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace YTINFOReader;

public class Plugin : BasePlugin<BasePluginConfiguration>
{
    public override string Name => Constants.PLUGIN_NAME;
    public static Plugin Instance { get; private set; }
    public override Guid Id => Guid.Parse(Constants.PLUGIN_GUID);
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }
}
