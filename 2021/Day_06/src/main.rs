fn main() {
    let initial_population = "3,4,3,1,2".split(',').map(|s| s.parse().unwrap()).collect();
    let iterations = 80;
    let population_size = population_size(initial_population, iterations);
    println!("Population size after {} days is:", iterations);
    println!("{}", population_size);
}

static ITERATIONS_BETWEEN_GROWTH: u8 = 6;
static INITIAL_TIME_UNTILL_GROWTH: u8 = 8;


fn population_size(initial_population: Vec<u8>, iterations: u8) -> u64 {
    let mut population_size: u64 = initial_population.len().try_into().unwrap();
    for individual in initial_population {
        // std::thread::Builder::new().stack_size(100000 *0xFF).spawn(move || println!("Number of children is {}", number_of_children_for_individual(individual, iterations))).unwrap().join();;
        population_size += number_of_children_for_individual(individual, iterations);
    }
    return population_size;
}

fn number_of_children_for_individual(iterations_until_split: u8, remaining_iterations: u8) -> u64 {
    let days_new_children_are_grown = get_days_individual_grows(iterations_until_split, remaining_iterations);

    let number_of_children: u64 = days_new_children_are_grown.len().try_into().unwrap();

    let mut number_of_sub_children: u64 = 0;

    for child_spawn_day in days_new_children_are_grown {
        let sub_children_of_child = number_of_children_for_individual(INITIAL_TIME_UNTILL_GROWTH, child_spawn_day);
        number_of_sub_children += sub_children_of_child;
    }

    return number_of_children + number_of_sub_children;
}

fn get_days_individual_grows(iterations_until_next_split: u8, remaining_iterations: u8) -> Vec<u8> {
    if remaining_iterations < iterations_until_next_split {
        return Vec::new();
    }
    let remaining_after_first = remaining_iterations - (iterations_until_next_split);
    // let mut counter = 0;
    let mut days_with_growth = Vec::new();
    for i in 0..remaining_after_first {
        if i % (ITERATIONS_BETWEEN_GROWTH + 1) == 0 {
            // counter += 1;
            days_with_growth.push(i + iterations_until_next_split + 1);
        }
    }
    return days_with_growth;
    // return ((remaining_after_first - 0) / (ITERATIONS_BETWEEN_GROWTH + 1)) as u64;
    // unimplemented!();
}

// mod data_types {
//     pub struct Growth {
//         pub day: u8,
//     }
// }


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
    fn test_get_days_individual_grows_case_0() {
        let result = get_days_individual_grows(0, 0);
        assert_eq!(result.len(), 0);
    }

    #[test]
    fn test_get_days_individual_grows_case_1() {
        let result = get_days_individual_grows(0, 1);
        assert_eq!(result.len(), 1, "Expected there to be 1 growth after 1 day when starting at 0.");
        assert_eq!(result[0], 1);
    }

    #[test]
    fn test_get_days_individual_grows_case_2() {
        let result = get_days_individual_grows(0, 6);
        assert_eq!(result.len(), 1, "Expected there to be 1 growth after 6 days when starting at 0.");
        assert_eq!(result[0], 1);
    }

    #[test]
    fn test_get_days_individual_grows_case_3() {
        let result = get_days_individual_grows(0, 7);
        assert_eq!(result.len(), 1, "Expected there to be 1 growth after 7 days when starting at 0.");
        assert_eq!(result[0], 1);
    }

    #[test]
    fn test_get_days_individual_grows_case_4() {
        let result = get_days_individual_grows(12, 2);
        assert_eq!(result.len(), 0);
    }

    #[test]
    fn test_get_days_individual_grows_case_5() {
        let result = get_days_individual_grows(0, 8);
        assert_eq!(result.len(), 2, "Expected there to be 2 growths after 8 days when starting at 0.");
        assert_eq!(result[0], 1);
        assert_eq!(result[1], 8);
    }

    #[test]
    fn test_get_days_individual_grows_case_6() {
        let result = get_days_individual_grows(0, 9);
        assert_eq!(result.len(), 2, "Expected there to be 2 growths after 9 days when starting at 0.");
        assert_eq!(result[0], 1);
        assert_eq!(result[1], 8);
    }

    #[test]
    fn test_get_days_individual_grows_case_7() {
        let result = get_days_individual_grows(0, 15);
        assert_eq!(result.len(), 3, "Expected there to be 3 growths after 15 days when starting at 0.");
        assert_eq!(result[0], 1);
        assert_eq!(result[1], 8);
        assert_eq!(result[2], 15);
    }

    #[test]
    fn test_get_days_individual_grows_starting_at_6_case_0() {
        let result = get_days_individual_grows(6, 6);
        assert_eq!(result.len(), 0, "Expected there to be 0 growths after 6 days when starting at 6.");
    }

    #[test]
    fn test_get_days_individual_grows_starting_at_6_case_1() {
        let result = get_days_individual_grows(6, 7);
        assert_eq!(result.len(), 1, "Expected there to be 1 growths after 7 days when starting at 6.");
        assert_eq!(result[0], 7);
    }

    #[test]
    fn does_mod_work_as_expected() {
        assert_eq!(0 % 7, 0);
        assert_eq!(7 % 7, 0);
        assert_eq!(1 % 7, 1);
    }
}
