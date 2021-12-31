fn main() {
    println!("Hello, world!");
}

fn read_input_from_file(filename: &str) -> Vec<Command> {
    let content = std::fs::read_to_string(filename)
        .expect("Failed to read from file");
    let lines = content.lines();
    let commands = lines.map(|s| make_command(s));
    return commands.collect();
}

fn parse_all_commands(input: &str) -> Vec<Command> {
    let lines = input.lines();
    let commands = lines.map(|s| make_command(s));
    return commands.collect();
}

fn make_command(input: &str) -> Command {
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
        // _ => panic!("Received \"{}\", which is not a recognized command.", _),
        _ => panic!("Received a not recognized command."),
    }
}

struct Command {
    action: Action,
    value: u64,
}

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

    const EXPECTED_RESULT: u64 = 150;
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
        let result = parse_action(testinput);
    }

    #[test]
    fn test_make_command() {
        let testinput = "forward 5";
        let result = make_command(testinput);
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
}
