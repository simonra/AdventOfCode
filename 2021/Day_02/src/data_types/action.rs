// Derive attribute allows enum to be printed using fmt::Debug.
#[derive(Debug)]
pub enum Action {
    Forward,
    Down,
    Up,
}
