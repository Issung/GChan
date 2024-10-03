using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GChan.Trackers
{
    public enum AssetType
    {
        Upload,
        Thumbnail,
    }

    // TODO: This should probably be merged into IDownloadable. Or IDownloadable should become IAsset. The AssetId concept is nice and should remain I think.
    // TODO: Should IAsset/IDownloadable be IEquatable<AssetId> (does that even work?)?
    public interface IAsset : IDownloadable
    {
        public AssetId Id { get; }
    }

    /// <summary>
    /// Act as the unique identifier for an asset.
    /// </summary>
    [JsonConverter(typeof(AssetIdJsonConverter))]
    public class AssetId : IEquatable<AssetId>
    {
        public AssetType Type { get; set; }
        
        /// <summary>
        /// May contain anything but a colon ":".
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Expecting the string in format "Type:Identifier"
        /// </summary>
        public AssetId(string str)
        {
            var parts = str.Split(':');

            Type = (AssetType)Enum.Parse(typeof(AssetType), parts[0]);
            Identifier = ValidateIdentifier(parts[1]);
        }

        /// <param name="identifier">May contain anything but a colon ":".</param>
        public AssetId(AssetType type, string identifier)
        {
            this.Type = type;
            this.Identifier = ValidateIdentifier(identifier);
        }

        public bool Equals(AssetId other)
        {
            return Type == other.Type && Identifier == other.Identifier;
        }

        public override int GetHashCode()
        {
            // Using prime numbers for a good distribution of hash codes
            int hash = 17;
            hash = hash * 31 + Type.GetHashCode();
            hash = hash * 31 + (Identifier != null ? Identifier.GetHashCode() : 0);
            return hash;
        }

        public override string ToString()
        {
            return $"{Type}:{Identifier}";
        }

        private string ValidateIdentifier(string identifier)
        {
            if (identifier.Contains(":"))
            {
                throw new Exception("AssetId identifiers may not contain a colon.");
            }

            return identifier;
        }
    }

    public class AssetIdJsonConverter : JsonConverter<AssetId>
    {
        public override AssetId Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            return new AssetId(stringValue);
        }

        public override void Write(Utf8JsonWriter writer, AssetId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
