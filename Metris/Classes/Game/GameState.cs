local Block = require("Blocks")
local PlayingField = require("PlayingField")
local BlockQueue = require("BlockQueue")

local GameState = {}
GameState.__index = GameState

function GameState.new()
    local self = setmetatable({}, GameState)

    self.currentBlock = nil
    self.playingField = PlayingField.new(22, 10)
    self.blockQueue = BlockQueue.new()
    self.gameOver = false
    self.score = 0
    self.heldBlock = nil
    self.canHold = true

    self:init()

    return self
end

function GameState:init()
    self.currentBlock = self.blockQueue:getAndUpdate()
    self.canHold = true
end

function GameState:holdBlock()
    if not self.canHold then
        return
    end

    if not self.heldBlock then
        self.heldBlock = self.currentBlock
        self.currentBlock = self.blockQueue:getAndUpdate()
    else
        local tmp = self.currentBlock
        self.currentBlock = self.heldBlock
        self.heldBlock = tmp
    end
end

function GameState:rotateBlockCW()
    self.currentBlock:rotateCW()

    if not self:blockFits() then
        self.currentBlock:rotateACW()
    end
end

function GameState:rotateBlockACW()
    self.currentBlock:rotateACW()

    if not self:blockFits() then
        self.currentBlock:rotateCW()
    end
end

function GameState:moveBlockLeft()
    self.currentBlock:move(0, -1)

    if not self:blockFits() then
        self.currentBlock:move(0, 1)
    end
end

function GameState:moveBlockRight()
    self.currentBlock:move(0, 1)

    if not self:blockFits() then
        self.currentBlock:move(0, -1)
    end
end

function GameState:moveBlockDown()
    self.currentBlock:move(1, 0)

    if not self:blockFits() then
        self.currentBlock:move(-1, 0)
        self:placeBlock()
    end
end

function GameState:blockFits()
    for _, p in ipairs(self.currentBlock:tilePositions()) do
        if not self.playingField:isEmpty(p.row, p.column) then
            return false
        end
    end
    return true
end

function GameState:isGameOver()
    return not (self.playingField:isRowEmpty(0) and self.playingField:isRowEmpty(1))
end

function GameState:placeBlock()
    for _, p in ipairs(self.currentBlock:tilePositions()) do
        self.playingField[p.row][p.column] = self.currentBlock.id
    end

    self.score = self.score + self.playingField:clearFullRows()

    if self:isGameOver() then
        self.gameOver = true
    else
        self.currentBlock = self.blockQueue:getAndUpdate()
        self.canHold = true
    end
end

function GameState:blockDropDistance()
    local drop = self.playingField.rows

    for _, p in ipairs(self.currentBlock:tilePositions()) do
        drop = math.min(drop, self:tileDropDistance(p))
    end

    return drop
end

function GameState:tileDropDistance(position)
    local drop = 0

    while self.playingField:isEmpty(position.row + drop + 1, position.column) do
        drop = drop + 1
    end

    return drop
end

function GameState:dropBlock()
    self.currentBlock:move(self:blockDropDistance(), 0)
    self:placeBlock()
end

return GameState
