use super::board_id::*;

#[derive(Debug, Clone, Copy)]
pub struct BoardEntry {
    pub board_id: BoardId,
    pub x: u8,
    pub y: u8,
    pub value: u8,
    pub marked: bool,
}
