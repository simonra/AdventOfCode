pub fn using_simple_for_loop(values: &Vec<u64>) -> u64 {
    let mut number_of_increases: u64 = 0;
    // use std::convert::TryInto;
    let number_of_values = values.len().try_into().unwrap();
    // Skip first element by iterating from 1 instead of 0.
    for n in 1..number_of_values {
        if values[n] > values[n-1] {
            number_of_increases += 1;
        }
    }
    return number_of_increases;
}

#[allow(dead_code)] // Don't complain about alternative implementation not being used.
pub fn using_window_slices(values: &Vec<u64>) -> u64 {
    // Trying out how windows work.
    // let mut number_of_increases: u64 = 0;
    // // values.windows(2).all(|window: [u64; 2]| -> () { if window[0] < window[1] { number_of_increases += 1; } });
    // let mut iter = values.windows(2);
    // for elem in iter {
    //     println!("first: {}, second: {}", elem[0], elem[1]);
    // }
    // return number_of_increases;

    // Verify that source of error E0308 expected `u64`, found `()` is not me misunderstanding something very basic.
    // return values.windows(2).fold(0u64, |number_of_increases: u64, next_window| {
    //     number_of_increases + next_window[0] + next_window[1]
    // });

    // Brief explanation:
    // `windows(2)`, splits vector into slices, called windows, of lenght 2.
    // Example `vec![1, 2, 3, 4].windows(2)` yields `[1, 2], [2, 3], [3, 4]`.
    // The returns are neccessary, because fold seems to expect the comprehension to return the next value to be added to the accumulation variable.
    return values.windows(2).fold(0, |number_of_increases, next_window| {
        if next_window[0] < next_window[1] {
            return number_of_increases + 1;
        }
        else {
            return number_of_increases;
        }
    });
}

#[cfg(test)]
mod tests {
    use super::*;
    static EXAMPLE_VALUES: [u64; 10] = [
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

    const EXPECTED_RESULT: u64 = 7;

    #[test]
    fn test_simple_for_loop() {
        let input = EXAMPLE_VALUES.to_vec();
        let result = using_simple_for_loop(&input);
        assert_eq!(result, EXPECTED_RESULT);
    }

    #[test]
    fn test_window_slices() {
        let input = EXAMPLE_VALUES.to_vec();
        let result = using_window_slices(&input);
        assert_eq!(result, EXPECTED_RESULT);
    }
}
