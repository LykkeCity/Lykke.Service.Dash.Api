namespace Lykke.Service.Dash.Api.Core.InsightClient
{
    public class BlocksInfo
    {
        public Block[] Blocks { get; set; }
    }

    public class Block
    {
        public long Height { get; set; }
    }
}
