use super::board_id::*;

#[derive(Debug, Copy, Clone)]
pub struct Board {
    pub board_id: BoardId,
    pub size_x: u8,
    pub size_y: u8,
    pub is_won: bool,
    pub score: u64,
}
