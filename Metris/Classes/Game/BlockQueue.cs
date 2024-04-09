use crate::blocks::{Block, LongBlock, JBlock, LBlock, SquareBlock, SquigglyBlock, TBlock, ReverseSquigglyBlock};
use rand::prelude::*;

pub struct BlockQueue {
    blocks: Vec<Box<dyn Block>>,
    rng: ThreadRng,
    next_block: Box<dyn Block>,
}

impl BlockQueue {
    pub fn new() -> Self {
        let mut rng = thread_rng();
        let blocks: Vec<Box<dyn Block>> = vec![
            Box::new(LongBlock::new()),
            Box::new(JBlock::new()),
            Box::new(LBlock::new()),
            Box::new(SquareBlock::new()),
            Box::new(SquigglyBlock::new()),
            Box::new(TBlock::new()),
            Box::new(ReverseSquigglyBlock::new()),
        ];
        let next_block = blocks[rng.gen_range(0, blocks.len())].clone();

        BlockQueue { blocks, rng, next_block }
    }

    pub fn get_and_update(&mut self) -> Box<dyn Block> {
        let block = self.next_block.clone();
        self.next_block = self.random_block();

        block
    }

    fn random_block(&mut self) -> Box<dyn Block> {
        let mut block = self.blocks[self.rng.gen_range(0, self.blocks.len())].clone();
        
        // Ensure the next block is different from the current one (if needed)
        while block.id() == self.next_block.id() {
            block = self.blocks[self.rng.gen_range(0, self.blocks.len())].clone();
        }

        block
    }
}
