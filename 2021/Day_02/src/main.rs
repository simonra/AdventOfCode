fn main() {
    println!("Hello, world!");
}

fn read_input_from_file(filename: &str) -> Vec<Command> {
    let content = std::fs::read_to_string(filename)
        .expect("Failed to read from file");
    let lines = content.lines();
    let commands = lines.map(|s| make_command(s));
    return commands.collect();

    // return std::fs::read_to_string(filename)
    //     .expect("Failed to read from file")
    //     .lines()
    //     .map(|s| make_command(s))
    //     .collect();
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
    fn test_parse_action_up() {
        let testinput = "up";
        let result = parse_action(testinput);
        assert!(matches!(result, Action::Up));
    }

    #[test]
    fn test_parse_action_down() {
        let testinput = "down";
        let result = parse_action(testinput);
        assert!(matches!(result, Action::Down));
    }

    #[test]
    fn test_parse_action_forward() {
        let testinput = "forward";
        let result = parse_action(testinput);
        assert!(matches!(result, Action::Forward));
    }

    #[test]
    #[should_panic]
    fn test_parse_action_fail() {
        let testinput = "";
        let result = parse_action(testinput);
    }

    // #[test]
    // fn test_window_slices() {
    //     // let input = EXAMPLE_VALUES.to_vec();
    //     // let result = using_window_slices(&input);
    //     // assert_eq!(result, EXPECTED_RESULT);
    // }
}


// read input -> split to lines -> map each line to tuple (where the last substring is mapped/parsed to int -> collect to liar of tuples.)
