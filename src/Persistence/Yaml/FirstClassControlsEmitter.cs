// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using CAT.PowerApps.Persistence.Models;
using CAT.PowerApps.Persistence.Templates;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace CAT.PowerApps.Persistence.Yaml;

internal class FirstClassControlsEmitter : ChainedEventEmitter
{
    private readonly IControlTemplateStore _controlTemplateStore;

    public FirstClassControlsEmitter(IEventEmitter nextEmitter, IControlTemplateStore controlTemplateStore)
       : base(nextEmitter)
    {
        _controlTemplateStore = controlTemplateStore ?? throw new ArgumentNullException(nameof(controlTemplateStore));
    }

    public override void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
    {
        nextEmitter.Emit(eventInfo, emitter);

        if (CheckIsFirstClass(eventInfo, out var nodeName))
        {
            var keySource = new ObjectDescriptor(nodeName, typeof(string), typeof(string));
            nextEmitter.Emit(new ScalarEventInfo(keySource), emitter);

            var valueSource = new ObjectDescriptor(null, typeof(string), typeof(string));
            nextEmitter.Emit(new ScalarEventInfo(valueSource), emitter);
        }
    }

    private bool CheckIsFirstClass(EventInfo eventInfo, [MaybeNullWhen(false)] out string nodeName)
    {
        if (_controlTemplateStore.TryGetById(((Control)eventInfo.Source.Value!).TemplateId, out var controlTemplate))
        {
            nodeName = controlTemplate.DisplayName;
            return true;
        }

        nodeName = null;
        return false;
    }
}
