mod count_increases_from_previous_value;

#[cfg(test)]
mod tests {
    // let example_values: Vec<u64> = vec![
    //     199,
    //     200,
    //     208,
    //     210,
    //     200,
    //     207,
    //     240,
    //     269,
    //     260,
    //     263
    // ];

    #[test]
    fn test_simple_for_loop() {
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

        let result = count_increases_from_previous_value::using_simple_for_loop(&example_values);
        assert_eq!(result, 7);
    }
}
