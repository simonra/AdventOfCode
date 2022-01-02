use super::board_id::*;

#[derive(Debug)]
pub struct Board {
    pub board_id: BoardId,
    pub size_x: u8,
    pub size_y: u8,
}
