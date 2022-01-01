mod data_types;
use crate::data_types::command::*;

mod input_handling;
use crate::input_handling::*;

mod calculate_position;
use crate::calculate_position::*;

fn main() {
    let commands = read_input_from_file("./src/input.txt");
    let final_position = calculate_position(&commands);
    let product = final_position.horizontal * final_position.depth;
    println!("Product of final position depth and horizontal position is:");
    println!("{}", product);
}

fn read_input_from_file(filename: &str) -> Vec<Command> {
    let content = std::fs::read_to_string(filename)
        .expect("Failed to read from file");
    return parse_all_commands(&content);
}
