// Derive attribute allows struct to be printed using fmt::Debug.
#[derive(Debug)]
pub struct Position {
    pub horizontal: u64,
    pub depth: u64,
}
