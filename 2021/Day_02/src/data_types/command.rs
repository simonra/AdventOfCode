use super::action::*;

#[derive(Debug)]
pub struct Command {
    pub action: Action,
    pub value: u64,
}
