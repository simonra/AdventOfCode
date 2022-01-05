use std::collections::HashMap;

mod un_recursive_attempt;

fn main() {
    let initial_population: Vec<u8> = "2,1,1,1,1,1,1,5,1,1,1,1,5,1,1,3,5,1,1,3,1,1,3,1,4,4,4,5,1,1,1,3,1,3,1,1,2,2,1,1,1,5,1,1,1,5,2,5,1,1,2,1,3,3,5,1,1,4,1,1,3,1,1,1,1,1,1,1,1,1,1,1,1,4,1,5,1,2,1,1,1,1,5,1,1,1,1,1,5,1,1,1,4,5,1,1,3,4,1,1,1,3,5,1,1,1,2,1,1,4,1,4,1,2,1,1,2,1,5,1,1,1,5,1,2,2,1,1,1,5,1,2,3,1,1,1,5,3,2,1,1,3,1,1,3,1,3,1,1,1,5,1,1,1,1,1,1,1,3,1,1,1,1,3,1,1,4,1,1,3,2,1,2,1,1,2,2,1,2,1,1,1,4,1,2,4,1,1,4,4,1,1,1,1,1,4,1,1,1,2,1,1,2,1,5,1,1,1,1,1,5,1,3,1,1,2,3,4,4,1,1,1,3,2,4,4,1,1,3,5,1,1,1,1,4,1,1,1,1,1,5,3,1,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,1,1,1,1,1,1,1,1,5,1,4,4,1,1,1,1,1,1,1,1,3,1,3,1,4,1,1,2,2,2,1,1,2,1,1".split(',').map(|s| s.parse().unwrap()).collect();
    let iterations = 80;
    let population_size = un_recursive_attempt::queued_population_calculation(initial_population.clone(), iterations);
    println!("Population size after {} days is:", iterations);
    println!("{}", population_size);

    // let final_contributions_for_individual = final_population_sizes_given_single_individual(80);
    // let sum = initial_population.clone().iter().fold(0, |sum, starting_individual| sum + final_contributions_for_individual[starting_individual]);
    // println!("New sum {}", sum);

    // print_upto_n();

    // let new_s = population_size_contribution_of_individuals_per_day(81);
    // println!("{:?}", new_s);
    // let final_contributions_for_individual = final_population_sizes_given_single_individual(80);
    // let sum = initial_population.clone().iter().fold(0u64, |sum, starting_individual| sum + new_s[&80][starting_individual]);
    // println!("New sum {}", sum);

    let population_map = population_size_contribution_of_individuals_per_day(257);
    // println!("{:?}", new_s);
    // let final_contributions_for_individual = final_population_sizes_given_single_individual(80);
    let new_population_size = initial_population.clone().iter().fold(0u64, |sum, starting_individual| sum + population_map[&256][starting_individual]);
    println!("Population size after {} days is:", 256);
    println!("{}", new_population_size);
    // println!("New sum {}", sum);

    // let new_iterations = 256;
    // let experimental_population = "0".split(',').map(|s| s.parse().unwrap()).collect::<Vec<u8>>();
    // let new_population_size = un_recursive_attempt::brute_force_population_size(experimental_population, new_iterations);

    // println!("Population size after {} days is:", new_iterations);
    // println!("{}", new_population_size);
}

fn print_upto_n(){
    for iteration in 0..20 {
        for starting_count in 0..9 {
            let population_size = un_recursive_attempt::queued_population_calculation(vec![starting_count], iteration);
            println!("Iteration {}, start count {}, population {}",
                iteration,
                starting_count,
                population_size,
            );
        }
        println!();
    }
}

fn final_population_sizes_given_single_individual(remaining_iterations: u16) -> HashMap<u8, u64> {
    let mut map: HashMap<u8, u64> = HashMap::new();
    for i in 0..7 {
        let population_size = un_recursive_attempt::queued_population_calculation(vec![i], remaining_iterations);
        map.insert(i, population_size);

        println!("Found that population size after {} days when starting with an indivudual with couter {} will be {}.", remaining_iterations, i, population_size);
    }
    return map;
}

fn population_size_contribution_of_individuals_per_day(number_of_days: u16) -> HashMap<u16, HashMap<u8, u64>> {
    // let mut population_contribution_for_counter_per_day = HashMap::new();
    let mut population_contribution_for_counter_per_day = initialize_map();
    for day_number in 1..number_of_days {
        for counter_for_individual in 0..9 {
            let growth_days = get_days_individual_grows(counter_for_individual.try_into().unwrap(), day_number);
            let mut number_of_children = 0;
            for child_spawn_day in growth_days {
                number_of_children += population_contribution_for_counter_per_day[&(day_number - child_spawn_day as u16)][&8];
            }
            // population_contribution_for_counter_per_day[&day_number].insert(HashMap::from([(0, 1)]));
            // let new_entry = HashMap::from([(counter_for_individual, number_of_children + 1)]);

            population_contribution_for_counter_per_day.entry(day_number).or_default().entry(counter_for_individual).or_insert(number_of_children + 1);
            // population_contribution_for_counter_per_day.insert(day_number, new_entry);
            // if counter_for_individual == 0  {
            //     let next_value: u64 = population_contribution_for_counter_per_day[&(day_number - 1)][&6] + 1;
            //     population_contribution_for_counter_per_day.insert(day_number, HashMap::from([(counter_for_individual, next_value)]));
            // }
            // let contribution = un_recursive_attempt::queued_population_calculation
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
    // population_contribution_for_counter_per_day.insert(0, HashMap::from([(1, 1)]));
    // population_contribution_for_counter_per_day.insert(0, HashMap::from([(2, 1)]));
    // population_contribution_for_counter_per_day.insert(0, HashMap::from([(3, 1)]));
    // population_contribution_for_counter_per_day.insert(0, HashMap::from([(4, 1)]));
    // population_contribution_for_counter_per_day.insert(0, HashMap::from([(5, 1)]));
    // population_contribution_for_counter_per_day.insert(0, HashMap::from([(6, 1)]));
    // population_contribution_for_counter_per_day.insert(0, HashMap::from([(7, 1)]));
    // population_contribution_for_counter_per_day.insert(0, HashMap::from([(8, 1)]));

    // population_contribution_for_counter_per_day.insert(1, HashMap::from([(0, 2)]));
    // population_contribution_for_counter_per_day.insert(1, HashMap::from([(1, 1)]));
    // population_contribution_for_counter_per_day.insert(1, HashMap::from([(2, 1)]));
    // population_contribution_for_counter_per_day.insert(1, HashMap::from([(3, 1)]));
    // population_contribution_for_counter_per_day.insert(1, HashMap::from([(4, 1)]));
    // population_contribution_for_counter_per_day.insert(1, HashMap::from([(5, 1)]));
    // population_contribution_for_counter_per_day.insert(1, HashMap::from([(6, 1)]));
    // population_contribution_for_counter_per_day.insert(1, HashMap::from([(7, 1)]));
    // population_contribution_for_counter_per_day.insert(1, HashMap::from([(8, 1)]));

    // population_contribution_for_counter_per_day.insert(2, HashMap::from([(0, 2)]));
    // population_contribution_for_counter_per_day.insert(2, HashMap::from([(1, 2)]));
    // population_contribution_for_counter_per_day.insert(2, HashMap::from([(2, 1)]));
    // population_contribution_for_counter_per_day.insert(2, HashMap::from([(3, 1)]));
    // population_contribution_for_counter_per_day.insert(2, HashMap::from([(4, 1)]));
    // population_contribution_for_counter_per_day.insert(2, HashMap::from([(5, 1)]));
    // population_contribution_for_counter_per_day.insert(2, HashMap::from([(6, 1)]));
    // population_contribution_for_counter_per_day.insert(2, HashMap::from([(7, 1)]));
    // population_contribution_for_counter_per_day.insert(2, HashMap::from([(8, 1)]));

    // population_contribution_for_counter_per_day.insert(3, HashMap::from([(0, 2)]));
    // population_contribution_for_counter_per_day.insert(3, HashMap::from([(1, 2)]));
    // population_contribution_for_counter_per_day.insert(3, HashMap::from([(2, 2)]));
    // population_contribution_for_counter_per_day.insert(3, HashMap::from([(3, 1)]));
    // population_contribution_for_counter_per_day.insert(3, HashMap::from([(4, 1)]));
    // population_contribution_for_counter_per_day.insert(3, HashMap::from([(5, 1)]));
    // population_contribution_for_counter_per_day.insert(3, HashMap::from([(6, 1)]));
    // population_contribution_for_counter_per_day.insert(3, HashMap::from([(7, 1)]));
    // population_contribution_for_counter_per_day.insert(3, HashMap::from([(8, 1)]));

    // population_contribution_for_counter_per_day.insert(4, HashMap::from([(0, 2)]));
    // population_contribution_for_counter_per_day.insert(4, HashMap::from([(1, 2)]));
    // population_contribution_for_counter_per_day.insert(4, HashMap::from([(2, 2)]));
    // population_contribution_for_counter_per_day.insert(4, HashMap::from([(3, 2)]));
    // population_contribution_for_counter_per_day.insert(4, HashMap::from([(4, 1)]));
    // population_contribution_for_counter_per_day.insert(4, HashMap::from([(5, 1)]));
    // population_contribution_for_counter_per_day.insert(4, HashMap::from([(6, 1)]));
    // population_contribution_for_counter_per_day.insert(4, HashMap::from([(7, 1)]));
    // population_contribution_for_counter_per_day.insert(4, HashMap::from([(8, 1)]));

    // population_contribution_for_counter_per_day.insert(5, HashMap::from([(0, 2)]));
    // population_contribution_for_counter_per_day.insert(5, HashMap::from([(1, 2)]));
    // population_contribution_for_counter_per_day.insert(5, HashMap::from([(2, 2)]));
    // population_contribution_for_counter_per_day.insert(5, HashMap::from([(3, 2)]));
    // population_contribution_for_counter_per_day.insert(5, HashMap::from([(4, 2)]));
    // population_contribution_for_counter_per_day.insert(5, HashMap::from([(5, 1)]));
    // population_contribution_for_counter_per_day.insert(5, HashMap::from([(6, 1)]));
    // population_contribution_for_counter_per_day.insert(5, HashMap::from([(7, 1)]));
    // population_contribution_for_counter_per_day.insert(5, HashMap::from([(8, 1)]));

    // population_contribution_for_counter_per_day.insert(6, HashMap::from([(0, 2)]));
    // population_contribution_for_counter_per_day.insert(6, HashMap::from([(1, 2)]));
    // population_contribution_for_counter_per_day.insert(6, HashMap::from([(2, 2)]));
    // population_contribution_for_counter_per_day.insert(6, HashMap::from([(3, 2)]));
    // population_contribution_for_counter_per_day.insert(6, HashMap::from([(4, 2)]));
    // population_contribution_for_counter_per_day.insert(6, HashMap::from([(5, 2)]));
    // population_contribution_for_counter_per_day.insert(6, HashMap::from([(6, 1)]));
    // population_contribution_for_counter_per_day.insert(6, HashMap::from([(7, 1)]));
    // population_contribution_for_counter_per_day.insert(6, HashMap::from([(8, 1)]));

    // population_contribution_for_counter_per_day.insert(7, HashMap::from([(0, 2)]));
    // population_contribution_for_counter_per_day.insert(7, HashMap::from([(1, 2)]));
    // population_contribution_for_counter_per_day.insert(7, HashMap::from([(2, 2)]));
    // population_contribution_for_counter_per_day.insert(7, HashMap::from([(3, 2)]));
    // population_contribution_for_counter_per_day.insert(7, HashMap::from([(4, 2)]));
    // population_contribution_for_counter_per_day.insert(7, HashMap::from([(5, 2)]));
    // population_contribution_for_counter_per_day.insert(7, HashMap::from([(6, 2)]));
    // population_contribution_for_counter_per_day.insert(7, HashMap::from([(7, 1)]));
    // population_contribution_for_counter_per_day.insert(7, HashMap::from([(8, 1)]));

    // population_contribution_for_counter_per_day.insert(8, HashMap::from([(0, 3)]));
    // population_contribution_for_counter_per_day.insert(8, HashMap::from([(1, 2)]));
    // population_contribution_for_counter_per_day.insert(8, HashMap::from([(2, 2)]));
    // population_contribution_for_counter_per_day.insert(8, HashMap::from([(3, 2)]));
    // population_contribution_for_counter_per_day.insert(8, HashMap::from([(4, 2)]));
    // population_contribution_for_counter_per_day.insert(8, HashMap::from([(5, 2)]));
    // population_contribution_for_counter_per_day.insert(8, HashMap::from([(6, 2)]));
    // population_contribution_for_counter_per_day.insert(8, HashMap::from([(7, 2)]));
    // population_contribution_for_counter_per_day.insert(8, HashMap::from([(8, 1)]));

    // population_contribution_for_counter_per_day.insert(9, HashMap::from([(0, 3)]));
    // population_contribution_for_counter_per_day.insert(9, HashMap::from([(1, 3)]));
    // population_contribution_for_counter_per_day.insert(9, HashMap::from([(2, 2)]));
    // population_contribution_for_counter_per_day.insert(9, HashMap::from([(3, 2)]));
    // population_contribution_for_counter_per_day.insert(9, HashMap::from([(4, 2)]));
    // population_contribution_for_counter_per_day.insert(9, HashMap::from([(5, 2)]));
    // population_contribution_for_counter_per_day.insert(9, HashMap::from([(6, 2)]));
    // population_contribution_for_counter_per_day.insert(9, HashMap::from([(7, 2)]));
    // population_contribution_for_counter_per_day.insert(9, HashMap::from([(8, 2)]));

    return population_contribution_for_counter_per_day;
}

// static ITERATIONS_BETWEEN_GROWTH: u8 = 6;
// static INITIAL_TIME_UNTILL_GROWTH: u8 = 8;


// fn population_size(initial_population: Vec<u8>, iterations: u8) -> u64 {
//     let mut population_size: u64 = initial_population.len().try_into().unwrap();
//     for individual in initial_population {
//         // std::thread::Builder::new().stack_size(100000 *0xFF).spawn(move || println!("Number of children is {}", number_of_children_for_individual(individual, iterations))).unwrap().join();;
//         population_size += number_of_children_for_individual(individual, iterations);
//     }
//     return population_size;
// }

// fn number_of_children_for_individual(iterations_until_split: u8, remaining_iterations: u8) -> u64 {
//     let days_new_children_are_grown = get_days_individual_grows(iterations_until_split, remaining_iterations);

//     let number_of_children: u64 = days_new_children_are_grown.len().try_into().unwrap();

//     let mut number_of_sub_children: u64 = 0;

//     for child_spawn_day in days_new_children_are_grown {
//         let sub_children_of_child = number_of_children_for_individual(INITIAL_TIME_UNTILL_GROWTH, child_spawn_day);
//         number_of_sub_children += sub_children_of_child;
//     }

//     return number_of_children + number_of_sub_children;
// }

// fn get_days_individual_grows(iterations_until_next_split: u8, remaining_iterations: u16) -> Vec<u8> {
//     if remaining_iterations < iterations_until_next_split {
//         return Vec::new();
//     }
//     let remaining_after_first = remaining_iterations - (iterations_until_next_split);
//     // let mut counter = 0;
//     let mut days_with_growth = Vec::new();
//     for i in 0..remaining_after_first {
//         if i % (ITERATIONS_BETWEEN_GROWTH + 1) == 0 {
//             // counter += 1;
//             days_with_growth.push(i + iterations_until_next_split + 1);
//         }
//     }
//     return days_with_growth;
//     // return ((remaining_after_first - 0) / (ITERATIONS_BETWEEN_GROWTH + 1)) as u64;
//     // unimplemented!();
// }

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

    // #[test]
    // fn test_population_size_18_iterations() {
    //     let initial_population = "3,4,3,1,2".split(',').map(|s| s.parse().unwrap()).collect();
    //     let iterations = 18;
    //     let result = population_size(initial_population, iterations);
    //     assert_eq!(result, 26);
    // }

    // #[test]
    // fn test_population_size_80_iterations() {
    //     let initial_population = "3,4,3,1,2".split(',').map(|s| s.parse().unwrap()).collect();
    //     let iterations = 80;
    //     let result = population_size(initial_population, iterations);
    //     assert_eq!(result, 5934);
    // }

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

    #[test]
    fn test_hashmap_works_as_expected() {
        let mut map: HashMap<u16, HashMap<u8, u64>> = HashMap::new();
        map.entry(0).or_default().entry(0).or_insert(3);
        assert_eq!(map[&0][&0], 3);

        map.entry(0).or_default().entry(1).or_insert(2);
        assert_eq!(map[&0][&1], 2);
    }

    #[test]
    fn test_population_size_contribution_of_individuals_per_day() {
        let result = population_size_contribution_of_individuals_per_day(81);
        assert_eq!(result.len(), 81);
        assert_eq!(result[&80][&0], 1421);
    }

    #[test]
    fn test_population_size_contribution_of_individuals_per_day_bigger() {
        let result = population_size_contribution_of_individuals_per_day(257);
        assert_eq!(result.len(), 257);
        assert_eq!(result[&80][&0], 1421);
    }
}
