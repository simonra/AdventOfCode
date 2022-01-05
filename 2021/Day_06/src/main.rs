use std::collections::HashMap;

fn main() {
    let starting_population = "2,1,1,1,1,1,1,5,1,1,1,1,5,1,1,3,5,1,1,3,1,1,3,1,4,4,4,5,1,1,1,3,1,3,1,1,2,2,1,1,1,5,1,1,1,5,2,5,1,1,2,1,3,3,5,1,1,4,1,1,3,1,1,1,1,1,1,1,1,1,1,1,1,4,1,5,1,2,1,1,1,1,5,1,1,1,1,1,5,1,1,1,4,5,1,1,3,4,1,1,1,3,5,1,1,1,2,1,1,4,1,4,1,2,1,1,2,1,5,1,1,1,5,1,2,2,1,1,1,5,1,2,3,1,1,1,5,3,2,1,1,3,1,1,3,1,3,1,1,1,5,1,1,1,1,1,1,1,3,1,1,1,1,3,1,1,4,1,1,3,2,1,2,1,1,2,2,1,2,1,1,1,4,1,2,4,1,1,4,4,1,1,1,1,1,4,1,1,1,2,1,1,2,1,5,1,1,1,1,1,5,1,3,1,1,2,3,4,4,1,1,1,3,2,4,4,1,1,3,5,1,1,1,1,4,1,1,1,1,1,5,3,1,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,1,1,1,1,1,1,1,1,5,1,4,4,1,1,1,1,1,1,1,1,3,1,3,1,4,1,1,2,2,2,1,1,2,1,1";

    let population_after_80_days = find_population_size(starting_population, 80);

    println!("Population size after 80 days is:");
    println!("{}", population_after_80_days);

    let population_after_256_days = find_population_size(starting_population, 256);

    println!("Population size after 256 days is:");
    println!("{}", population_after_256_days);
}

fn find_population_size(input_population: &str, number_of_days: u16) -> u64 {
    let initial_population: Vec<u8> = input_population.split(',').map(|s| s.parse().unwrap()).collect();
    let population_map = population_size_contribution_of_individuals_per_day(number_of_days + 1);
    return initial_population.iter().fold(0u64, |sum, starting_individual| sum + population_map[&number_of_days][starting_individual])
}

fn population_size_contribution_of_individuals_per_day(number_of_days: u16) -> HashMap<u16, HashMap<u8, u64>> {
    let mut population_contribution_for_counter_per_day = initialize_map();
    // Start iterating at day 1, because day 0 is already initialized.
    for day_number in 1..number_of_days {
        for counter_for_individual in 0..9 {
            let growth_days = get_days_individual_grows(counter_for_individual.try_into().unwrap(), day_number);
            let number_of_children = growth_days
                .iter()
                .fold(0u64, |sum, child_spawn_day|
                    {
                        // Look up the growth numbers for counter value 8 for the given day, because that is the internal counter for the new fish that would have spawned on this day.
                        return sum + population_contribution_for_counter_per_day[&(day_number - child_spawn_day)][&8]
                    }
                );

            // Add 1 to number of children to ensure that individual also gets counted.
            population_contribution_for_counter_per_day.entry(day_number).or_default().entry(counter_for_individual).or_insert(number_of_children + 1);
        }
    }

    return population_contribution_for_counter_per_day;
}

fn initialize_map() -> HashMap<u16, HashMap<u8, u64>> {
    let mut population_contribution_for_counter_per_day = HashMap::new();
    population_contribution_for_counter_per_day.insert(
        0,
        HashMap::from([
            (0, 1),
            (1, 1),
            (2, 1),
            (3, 1),
            (4, 1),
            (5, 1),
            (6, 1),
            (7, 1),
            (8, 1),
        ])
    );

    return population_contribution_for_counter_per_day;
}

fn get_days_individual_grows(iterations_until_next_split: u16, remaining_iterations: u16) -> Vec<u16> {
    if remaining_iterations < iterations_until_next_split {
        return Vec::new();
    }
    let remaining_after_first = remaining_iterations - (iterations_until_next_split);
    // let mut counter = 0;
    const ITERATIONS_BETWEEN_GROWTH: u16 = 6;
    let mut days_with_growth = Vec::new();
    for i in 0..remaining_after_first {
        if i % (ITERATIONS_BETWEEN_GROWTH + 1) == 0 {
            // counter += 1;
            days_with_growth.push(i + iterations_until_next_split + 1);
        }
    }
    return days_with_growth;
}


#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_population_size_18_iterations() {
        let input_population = "3,4,3,1,2";
        let iterations = 18;
        let result = find_population_size(input_population, iterations);
        assert_eq!(result, 26);
    }

    #[test]
    fn test_population_size_80_iterations() {
        let input_population = "3,4,3,1,2";
        let iterations = 80;
        let result = find_population_size(input_population, iterations);
        assert_eq!(result, 5934);
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
}
