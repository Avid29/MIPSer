// Adam Dernis 2024

using Microsoft.Extensions.Localization;
using System.Resources;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test.Assembler.MIPS")]
[assembly: InternalsVisibleTo("Test.Assembler.MIPS.Live")]

[assembly: ResourceLocation("Resources")]
[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.Satellite)]
