use super::board_id::*;
use super::board_entry::*;

#[derive(Debug, Clone)]
pub struct Board {
    pub board_id: BoardId,
    pub size_x: u8,
    pub size_y: u8,
    pub entries: Vec<Vec<BoardEntry>>,
    pub is_won: bool,
    pub score: u64,
}
