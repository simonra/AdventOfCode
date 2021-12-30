mod count_increases_from_previous_value;

fn main(){
    let values = read_input_from_file("./src/input.txt");
    let count = count_increases_from_previous_value::using_simple_for_loop(&values);
    println!("Day 1: Sonar Sweep results");
    println!("I have found that \"{}\" measurements are larger than the previous measurement", count);
}

fn read_input_from_file(filename: &str) -> Vec<u64> {
    let contents = std::fs::read_to_string(filename).expect("Failed to read from file");
    let mut lines = contents.lines();
    let numbers: Vec<u64> = lines.map(|s| s.parse().expect("Failed to parse line")).collect();
    return numbers;
}
