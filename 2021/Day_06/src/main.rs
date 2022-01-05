use std::collections::HashMap;

fn main() {
    let initial_population: Vec<u8> = "2,1,1,1,1,1,1,5,1,1,1,1,5,1,1,3,5,1,1,3,1,1,3,1,4,4,4,5,1,1,1,3,1,3,1,1,2,2,1,1,1,5,1,1,1,5,2,5,1,1,2,1,3,3,5,1,1,4,1,1,3,1,1,1,1,1,1,1,1,1,1,1,1,4,1,5,1,2,1,1,1,1,5,1,1,1,1,1,5,1,1,1,4,5,1,1,3,4,1,1,1,3,5,1,1,1,2,1,1,4,1,4,1,2,1,1,2,1,5,1,1,1,5,1,2,2,1,1,1,5,1,2,3,1,1,1,5,3,2,1,1,3,1,1,3,1,3,1,1,1,5,1,1,1,1,1,1,1,3,1,1,1,1,3,1,1,4,1,1,3,2,1,2,1,1,2,2,1,2,1,1,1,4,1,2,4,1,1,4,4,1,1,1,1,1,4,1,1,1,2,1,1,2,1,5,1,1,1,1,1,5,1,3,1,1,2,3,4,4,1,1,1,3,2,4,4,1,1,3,5,1,1,1,1,4,1,1,1,1,1,5,3,1,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,1,1,1,1,1,1,1,1,5,1,4,4,1,1,1,1,1,1,1,1,3,1,3,1,4,1,1,2,2,2,1,1,2,1,1".split(',').map(|s| s.parse().unwrap()).collect();

    let population_map = population_size_contribution_of_individuals_per_day(257);

    let population_after_80_days = initial_population.clone().iter().fold(0u64, |sum, starting_individual| sum + population_map[&80][starting_individual]);

    println!("Population size after 80 days is:");
    println!("{}", population_after_80_days);

    let population_after_256_days = initial_population.clone().iter().fold(0u64, |sum, starting_individual| sum + population_map[&256][starting_individual]);
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
    for day_number in 1..number_of_days {
        for counter_for_individual in 0..9 {
            let growth_days = get_days_individual_grows(counter_for_individual.try_into().unwrap(), day_number);
            let mut number_of_children = 0;
            for child_spawn_day in growth_days {
                number_of_children += population_contribution_for_counter_per_day[&(day_number - child_spawn_day as u16)][&8];
            }

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

static ITERATIONS_BETWEEN_GROWTH: u16 = 6;
fn get_days_individual_grows(iterations_until_next_split: u16, remaining_iterations: u16) -> Vec<u16> {
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

    // #[test]
    // fn test_population_size_one_individual() {
    //     let initial_population = "0".split(',').map(|s| s.parse().unwrap()).collect();
    //     let iterations = 1;
    //     let result = population_size(initial_population, iterations);
    //     assert_eq!(result, 2);
    // }

    // #[test]
    // fn test_population_size_one_individual_6_generations() {
    //     let initial_population = "0".split(',').map(|s| s.parse().unwrap()).collect();
    //     let iterations = 6;
    //     let result = population_size(initial_population, iterations);
    //     assert_eq!(result, 1);
    // }

    // #[test]
    // fn test_population_size_one_individual_7_generations() {
    //     let initial_population = "0".split(',').map(|s| s.parse().unwrap()).collect();
    //     let iterations = 15;
    //     let result = population_size(initial_population, iterations);
    //     assert_eq!(result, 2, "Expected population when starting with 1 individual at 0 after 7 days i 2.");
    // }

    // #[test]
    // fn test_population_size_one_individual_21_generations() {
    //     let initial_population = "0".split(',').map(|s| s.parse().unwrap()).collect();
    //     let iterations = 21;
    //     let result = population_size(initial_population, iterations);
    //     assert_eq!(result, 5);
    // }

    // #[test]
    // fn test_population_size_two_individual() {
    //     let initial_population = "0,0".split(',').map(|s| s.parse().unwrap()).collect();
    //     let iterations = 1;
    //     let result = population_size(initial_population, iterations);
    //     assert_eq!(result, 4);
    // }
}
