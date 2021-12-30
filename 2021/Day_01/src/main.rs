mod count_increases_from_previous_value;

fn main(){
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
    let count = count_increases_from_previous_value::using_simple_for_loop(&example_values);
    println!("Count is: \"{}\"", count);
}
