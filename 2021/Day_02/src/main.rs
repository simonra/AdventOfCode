fn main() {
    let commands = read_input_from_file("./src/input.txt");
    let final_position = calculate_position(&commands);
    let product = final_position.horizontal * final_position.depth;
    println!("Product of final position depth and horizontal position is:");
    println!("{}", product);
}

fn calculate_position(commands: &Vec<Command>) -> Position {
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
fn calculate_position_using_filter_compact(commands: &Vec<Command>) -> Position {
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
fn calculate_position_using_filter_verbose(commands: &Vec<Command>) -> Position {
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

fn read_input_from_file(filename: &str) -> Vec<Command> {
    let content = std::fs::read_to_string(filename)
        .expect("Failed to read from file");
    return parse_all_commands(&content);
}

fn parse_all_commands(input: &str) -> Vec<Command> {
    let lines = input.lines();
    let commands = lines.map(|s| parse_command(s));
    return commands.collect();
}

fn parse_command(input: &str) -> Command {
    let split: Vec<&str> = input.split_whitespace().take(2).collect();
    let command_action = parse_action(split[0]);
    let command_value = split[1].parse().expect("Failed to parse value from command.");
    return Command { action: command_action, value: command_value };
}

fn parse_action(input: &str) -> Action {
    match input {
        "forward" => Action::Forward,
        "down" => Action:: Down,
        "up" => Action::Up,
        _ => panic!("Received \"{}\", which is not a recognized command.", input),
    }
}

// Derive attribute allows struct to be printed using fmt::Debug.
#[derive(Debug)]
struct Position {
    horizontal: u64,
    depth: u64,
}

#[derive(Debug)]
struct Command {
    action: Action,
    value: u64,
}

// Derive attribute allows enum to be printed using fmt::Debug.
#[derive(Debug)]
enum Action {
    Forward,
    Down,
    Up,
}

#[cfg(test)]
mod tests {
    use super::*;

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
    fn test_parse_action_forward() {
        let testinput = "forward";
        let result = parse_action(testinput);
        assert!(matches!(result, Action::Forward));
    }

    #[test]
    fn test_parse_action_down() {
        let testinput = "down";
        let result = parse_action(testinput);
        assert!(matches!(result, Action::Down));
    }

    #[test]
    fn test_parse_action_up() {
        let testinput = "up";
        let result = parse_action(testinput);
        assert!(matches!(result, Action::Up));
    }

    #[test]
    #[should_panic]
    fn test_parse_action_fail() {
        let testinput = "";
        let _result = parse_action(testinput);
    }

    #[test]
    fn test_parse_command() {
        let testinput = "forward 5";
        let result = parse_command(testinput);
        assert_eq!(result.value, 5);
        assert!(matches!(result.action, Action::Forward));
    }

    #[test]
    fn test_parse_all_commands() {
        let testinput = get_test_input();
        let result = parse_all_commands(testinput);

        assert_eq!(result.len(), 6);

        assert_eq!(result[3].value, 3);
        assert!(matches!(result[3].action, Action::Up));
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
