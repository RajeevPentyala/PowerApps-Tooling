// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using CAT.PowerApps.Persistence.Collections;
using CAT.PowerApps.Persistence.Models;

namespace CAT.PowerApps.Persistence.Templates;

public interface IControlFactory
{
    Control Create(string name, string template, ControlPropertiesCollection? properties = null, IList<Control>? children = null);

    Control Create(string name, ControlTemplate template, ControlPropertiesCollection? properties = null, IList<Control>? children = null);

    App CreateApp(string name, ControlPropertiesCollection? properties = null);

    Screen CreateScreen(string name, ControlPropertiesCollection? properties = null, IList<Control>? children = null);
}
