using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Metris.Classes.Blocks;

namespace Metris.Classes.Game
{
    public class BlockQueue
    {
        // Instantiate all the possible block types
        private readonly Block[] blocks = new Block[]
        {
            new LongBlock(),
            new JBlock(),
            new LBlock(),
            new SquareBlock(),
            new SquigglyBlock(),
            new TBlock(),
            new ReverseSquigglyBlock()
        };

        private readonly Random random = new Random();

        public Block nextBlock { get; private set; }

        public BlockQueue()
        {
            nextBlock = RandomBlock();
        }

        private Block RandomBlock()
        {
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetAndUpdate()
        {
            Block block = nextBlock;

            do
            {
                nextBlock = RandomBlock();
            }
            while(block.Id == nextBlock.Id && false);

            return block;
        }
    }
}
