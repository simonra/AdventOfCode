use std::convert::TryInto;

fn main(){
    println!("hello world!!");
    let example_values: Vec<u64> = vec![
        199,
        200,
        208,
        210,
        200,
        207,
        240,
        269,
        260,
        263
    ];
    let count = count_increases_from_previous_value(example_values);
    println!("Count is: \"{}\"", count);
}

fn count_increases_from_previous_value(values: Vec<u64>) -> u64 {
    let mut number_of_increases: u64 = 0;
    let number_of_values = values.len().try_into().unwrap();
    // Skip first element by iterating from 1 instead of 0.
    for n in 1..number_of_values {
        if values[n] > values[n-1] {
            number_of_increases += 1;
        }
    }
    // for value in values.skip(1) {

    // }
    return number_of_increases;
}
