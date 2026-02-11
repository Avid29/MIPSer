// Avishai Dernis 2025

using MIPS.Assembler.Config;
using ObjectFiles.Elf.Config;
using RASM.Modules.Config;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Mipser.Config;

/// <summary>
/// A wrapper for the <see cref="AssemblerConfig"/> to allow polymorphic XML serialization.
/// </summary>
public class AssemblerConfigWrapper : IXmlSerializable
{
    /// <summary>
    /// Gets the config's type.
    /// </summary>
    [XmlAttribute("Type")]
    public string Type
    {
        get
        {
            return Config switch
            {
                RasmConfig => "RASM",
                AssemblerConfig or _ => "ELF", 
            };
        }
    }

    /// <summary>
    /// Gets the wrapped assembler config.
    /// </summary>
    public AssemblerConfig? Config { get; set; }

    /// <inheritdoc/>
    public XmlSchema? GetSchema() => null;

    /// <inheritdoc/>
    public void ReadXml(XmlReader reader)
    {
        var type = reader.GetAttribute(nameof(Type));

        reader.ReadStartElement();

        var inner = new XmlDocument();
        var fragment = inner.ReadNode(reader);
        if (fragment is null || type is null)
            return;

        Config = DeserializeConfig(fragment, type);

        // Read to end of element
        int startDepth = reader.Depth;
        while (reader.Depth >= startDepth)
            reader.Read();
    }

    /// <inheritdoc/>
    public void WriteXml(XmlWriter writer)
    {
        if (Config is null)
            return;

        writer.WriteAttributeString(nameof(Type), Type);

        var serializer = new XmlSerializer(Config.GetType());
        serializer.Serialize(writer, Config);
    }

    private static AssemblerConfig? DeserializeConfig(XmlNode node, string type)
    {
        return type switch
        {
            "RASM" => DeserializeNode<RasmConfig>(node),
            "ELF" => DeserializeNode<ElfConfig>(node),
            _ => throw new InvalidOperationException($"Unknown Type '{type}'"),
        };
    }

    private static T? DeserializeNode<T>(XmlNode node)
    {
        // Create synthetic root node of matching type
        var xml = $"<{typeof(T).Name}>{node.InnerText}</{typeof(T).Name}>";

        var serializer = new XmlSerializer(typeof(T));
        using var reader = new StringReader(xml);
        return (T?)serializer.Deserialize(reader);
    }
}
