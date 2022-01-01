use crate::data_types::action::*;
use crate::data_types::command::*;
use crate::data_types::position::*;

pub fn calculate_position_with_aim(commands: &Vec<Command>) -> Position {
    let mut position_depth = 0;
    let mut position_horizontal = 0;
    let mut aim = 0;
    for command in commands {
        match command.action {
            Action::Forward => { position_horizontal += command.value; position_depth += aim * command.value },
            Action::Down => aim += command.value,
            Action::Up => aim -= command.value,
        }
    }

    return Position { horizontal: position_horizontal, depth: position_depth, };
}

#[cfg(test)]
mod tests {
    use super::*;
    use crate::input_handling::*;

    fn get_test_input() -> &'static str {
        return "forward 5\n\
        down 5\n\
        forward 8\n\
        up 3\n\
        down 8\n\
        forward 2\n";
    }

    fn assert_expected_position(position: Position) {
        let expected = Position { horizontal: 15, depth: 60, };
        assert_eq!(position.horizontal, expected.horizontal);
        assert_eq!(position.depth, expected.depth);
    }

    #[test]
    fn test_calculate_position_with_aim() {
        let testinput = get_test_input();
        let commands = parse_all_commands(testinput);
        let result = calculate_position_with_aim(&commands);

        assert_expected_position(result);
    }
}
