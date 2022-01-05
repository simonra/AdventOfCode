fn main() {
    println!("Hello, world!");
}

static ITERATIONS_BETWEEN_GROWTH: u8 = 6;
static INITIAL_TIME_UNTILL_GROWTH: u8 = 8;


fn population_size(initial_population: Vec<u8>, iterations: u8) -> u64 {
    unimplemented!();
}

fn number_of_children_for_individual(iterations_until_split: u8, remaining_iterations: u8) -> u64 {
    let number_of_iterations_before_first_split = remaining_iterations - iterations_until_split;
    let remaining_regular_iterations_after_first_split = remaining_iterations - number_of_iterations_before_first_split;
    let number_of_divisions_in_this_individuals_lifetime = remaining_regular_iterations_after_first_split / ITERATIONS_BETWEEN_GROWTH;

    unimplemented!();
}

fn calculate_number_of_divisions(iterations_until_next_split: u8, remaining_iterations: u8) -> u64 {
    if remaining_iterations < iterations_until_next_split {
        return 0;
    }
    let remaining_after_first = remaining_iterations - (iterations_until_next_split);
    let mut counter = 0;
    for i in 0..remaining_after_first {
        if i % (ITERATIONS_BETWEEN_GROWTH + 1) == 0 {
            counter += 1;
        }
    }
    return counter;
    // return (remaining_after_first / ITERATIONS_BETWEEN_GROWTH) as u64;
    // unimplemented!();
}


#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_population_size_18_iterations() {
        let initial_population = "3,4,3,1,2".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 18;
        let result = population_size(initial_population, iterations);
        assert_eq!(result, 26);
    }

    #[test]
    fn test_population_size_80_iterations() {
        let initial_population = "3,4,3,1,2".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 80;
        let result = population_size(initial_population, iterations);
        assert_eq!(result, 5934);
    }

    #[test]
    fn test_population_size_one_individual() {
        let initial_population = "0".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 1;
        let result = population_size(initial_population, iterations);
        assert_eq!(result, 2);
    }

    #[test]
    fn test_population_size_one_individual_6_generations() {
        let initial_population = "0".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 6;
        let result = population_size(initial_population, iterations);
        assert_eq!(result, 1);
    }

    #[test]
    fn test_population_size_one_individual_7_generations() {
        let initial_population = "0".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 7;
        let result = population_size(initial_population, iterations);
        assert_eq!(result, 2);
    }

    #[test]
    fn test_population_size_one_individual_21_generations() {
        let initial_population = "0".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 21;
        let result = population_size(initial_population, iterations);
        assert_eq!(result, 5);
    }

    #[test]
    fn test_population_size_two_individual() {
        let initial_population = "0,0".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 1;
        let result = population_size(initial_population, iterations);
        assert_eq!(result, 4);
    }

    #[test]
    fn test_calculate_number_of_divisions_case_0() {
        let result = calculate_number_of_divisions(0, 0);
        assert_eq!(result, 0);
    }

    #[test]
    fn test_calculate_number_of_divisions_case_1() {
        let result = calculate_number_of_divisions(0, 1);
        assert_eq!(result, 1, "Expected there to be 1 growth after 1 day when starting at 0.");
    }

    #[test]
    fn test_calculate_number_of_divisions_case_2() {
        let result = calculate_number_of_divisions(0, 6);
        assert_eq!(result, 1, "Expected there to be 1 growth after 6 days when starting at 0.");
    }

    #[test]
    fn test_calculate_number_of_divisions_case_3() {
        let result = calculate_number_of_divisions(0, 7);
        assert_eq!(result, 1, "Expected there to be 1 growth after 7 days when starting at 0.");
    }

    #[test]
    fn test_calculate_number_of_divisions_case_4() {
        let result = calculate_number_of_divisions(12, 2);
        assert_eq!(result, 0);
    }

    #[test]
    fn test_calculate_number_of_divisions_case_5() {
        let result = calculate_number_of_divisions(0, 8);
        assert_eq!(result, 2, "Expected there to be 2 growths after 8 days when starting at 0.");
    }

    #[test]
    fn test_calculate_number_of_divisions_case_6() {
        let result = calculate_number_of_divisions(0, 9);
        assert_eq!(result, 2, "Expected there to be 2 growths after 9 days when starting at 0.");
    }

    #[test]
    fn test_calculate_number_of_divisions_case_7() {
        let result = calculate_number_of_divisions(0, 15);
        assert_eq!(result, 3, "Expected there to be 3 growths after 15 days when starting at 0.");
    }

    #[test]
    fn does_mod_work_as_expected() {
        assert_eq!(0 % 7, 0);
        assert_eq!(7 % 7, 0);
        assert_eq!(1 % 7, 1);
    }
}
