use crate::data_types::action::*;
use crate::data_types::command::*;
use crate::data_types::position::*;

pub fn calculate_position(commands: &Vec<Command>) -> Position {
    let mut position_depth = 0;
    let mut position_horizontal = 0;
    for command in commands {
        match command.action {
            Action::Forward => position_horizontal += command.value,
            Action::Down => position_depth += command.value,
            Action::Up => position_depth -= command.value,
        }
    }

    return Position { horizontal: position_horizontal, depth: position_depth, };
}

#[allow(dead_code)] // Don't complain about alternative implementation not being used.
pub fn calculate_position_using_filter_compact(commands: &Vec<Command>) -> Position {
    // Make vector into imutable slice so that it can safely be iterated over several times.
    let commands_slice = commands.as_slice();

    let sum_forwards: u64 = commands_slice
        .iter()
        .filter(|command| matches!(command.action, Action::Forward))
        .map(|command| command.value)
        .sum();

    let sum_downs: u64 = commands_slice
        .iter()
        .filter(|command| matches!(command.action, Action::Down))
        .map(|command| command.value)
        .sum();

    let sum_ups: u64 = commands_slice
        .iter()
        .filter(|command| matches!(command.action, Action::Up))
        .map(|command| command.value)
        .sum();

    let position_depth = sum_downs - sum_ups;

    return Position { horizontal: sum_forwards, depth: position_depth, };
}

#[allow(dead_code)] // Don't complain about alternative implementation not being used.
pub fn calculate_position_using_filter_verbose(commands: &Vec<Command>) -> Position {
    // Make vector into imutable slice so that it can safely be iterated over several times.
    let commands_slice = commands.as_slice();

    let forwards = commands_slice.iter().filter(|command| matches!(command.action, Action::Forward)).collect::<Vec<&Command>>();
    // println!("{:?}", forwards);

    let ups = commands_slice.iter().filter(|command| matches!(command.action, Action::Up)).collect::<Vec<&Command>>();

    let downs = commands_slice.iter().filter(|command| matches!(command.action, Action::Down)).collect::<Vec<&Command>>();

    let forwards_sum: u64 = forwards.iter().map(|command| command.value).sum();
    // println!("forwards sum: {}", forwards_sum);

    let downs_sum: u64 = downs.iter().map(|command| command.value).sum();
    // println!("forwards sum: {}", forwards_sum);

    let ups_sum: u64 = ups.iter().map(|command| command.value).sum();

    let position_depth = downs_sum - ups_sum;

    return Position { horizontal: forwards_sum, depth: position_depth, };
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
        let expected = Position { horizontal: 15, depth: 10, };
        assert_eq!(position.horizontal, expected.horizontal);
        assert_eq!(position.depth, expected.depth);
    }

    #[test]
    fn test_calculate_position() {
        let testinput = get_test_input();
        let commands = parse_all_commands(testinput);
        let result = calculate_position(&commands);

        assert_expected_position(result);
    }

    #[test]
    fn test_calculate_position_using_filter_compact() {
        let testinput = get_test_input();
        let commands = parse_all_commands(testinput);
        let result = calculate_position_using_filter_compact(&commands);

        assert_expected_position(result);
    }

    #[test]
    fn test_calculate_position_using_filter_verbose() {
        let testinput = get_test_input();
        let commands = parse_all_commands(testinput);
        let result = calculate_position_using_filter_verbose(&commands);

        assert_expected_position(result);
    }
}
