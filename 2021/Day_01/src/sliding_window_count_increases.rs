#[allow(dead_code)] // Don't complain about alternative implementation not being used.
pub fn using_simple_for_loop(values: &Vec<u64>) -> u64 {
    let mut number_of_increases: u64 = 0;
    // use std::convert::TryInto;
    let number_of_values: usize = values.len().try_into().unwrap();
    let sliding_window_size = 3;
    let last_index: usize = number_of_values - sliding_window_size;
    // Skip first element by iterating from 1 instead of 0.
    for n in 0..last_index {
        let sum_first_window = values[n+0] + values[n+1] + values[n+2];
        let sum_second_window = values[n+1] + values[n+2] + values[n+3];
        if sum_second_window > sum_first_window {
            number_of_increases += 1;
        }
    }
    return number_of_increases;
}

#[allow(dead_code)] // Don't complain about alternative implementation not being used.
pub fn using_window_slices(values: &Vec<u64>) -> u64 {
    return values.windows(4).fold(0, |number_of_increases, next_window| {
        let sum_first_window = next_window[0] + next_window[1] + next_window[2];
        let sum_second_window = next_window[1] + next_window[2] + next_window[3];
        if sum_first_window < sum_second_window {
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

    const EXPECTED_RESULT: u64 = 5;

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
