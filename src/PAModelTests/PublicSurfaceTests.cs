// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using System.Text;
using CAT.Formulas.Tools;

namespace PAModelTests;

[TestClass]
public class PublicSurfaceTests
{
    [TestMethod]
    public void TestNamespace()
    {
        var asm = typeof(CanvasDocument).Assembly;
        var publicNamespace = "CAT.Formulas.Tools";
        var sb = new StringBuilder();
        foreach (var type in asm.GetTypes().Where(t => t.IsPublic))
        {
            var name = type.FullName;
            name.Should().StartWith(publicNamespace, $"Type {name} is not in the public namespace {publicNamespace}");
        }
    }
}

