mod count_increases_from_previous_value;
mod sliding_window_count_increases;

fn main(){
    println!("Day 1: Sonar Sweep results");
    let values = read_input_from_file("./src/input.txt");
    let count = count_increases_from_previous_value::using_simple_for_loop(&values);
    println!("I have found that \"{}\" measurements are larger than the previous measurement", count);
    let sliding_window_count = sliding_window_count_increases::using_window_slices(&values);
    println!("Using a 3 measurement sliding window I found \"{}\" sums to be larger than the previous.", sliding_window_count);

}

fn read_input_from_file(filename: &str) -> Vec<u64> {
    // Could save some resources by simplifying this to a hughe one-liner,
    // but I figure that the way it stands now
    // it is easier for me to reuse parts when I learn more later.

    let contents = std::fs::read_to_string(filename).expect("Failed to read from file");
    let lines = contents.lines();
    let numbers: Vec<u64> = lines.map(|s| s.parse().expect("Failed to parse line")).collect();
    return numbers;
}
