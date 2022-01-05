pub fn brute_force_population_size(initial_population: Vec<u8>, iterations: u16) -> u64 {
    let mut population = initial_population.clone();
    for i in 0..iterations {
        for j in 0..population.len() {
            let individual_next_value: i16 = population[j] as i16 - 1;
            if individual_next_value < 0 {
                population[j] = 6;
                population.push(8);
            }
            else {
                population[j] = individual_next_value as u8;
            }
        }
        println!("Day {}, population is {}", i, population.len());
    }

    return population.len().try_into().unwrap();
}

pub fn queued_population_calculation(initial_population: Vec<u8>, iterations: u16) -> u64 {
    let mut population_size: u64 = initial_population.len().try_into().unwrap();

    let mut queue = initial_population.into_iter().map(|c| Lanternfish {counter: c, available_days: iterations}).collect::<Vec<Lanternfish>>();

    let mut counter: u64 = 0;

    while queue.len() > 0 {
        let next_fish = queue.pop().unwrap();
        let days_with_spawn = get_days_individual_grows(next_fish.counter.into(), next_fish.available_days);

        // Add number of children, + 1 for iteself
        population_size += days_with_spawn.len() as u64;
        for day in days_with_spawn {
            queue.push(Lanternfish {counter: 8, available_days: next_fish.available_days - day});
        }

        counter += 1; // This ends up also tracking the number of fish.
        // if counter % 1024 == 0
        // if queue.len() < 150
        // {
        //     println!("Processing the {}th fish. Queue size is {}, population size {}", counter, queue.len(), population_size);

        // }
    }

    return population_size;
}

#[derive(Debug, Copy, Clone)]
struct Lanternfish {
    counter: u8,
    available_days: u16,
    // number_of_children: u64,
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
    // return ((remaining_after_first - 0) / (ITERATIONS_BETWEEN_GROWTH + 1)) as u64;
    // unimplemented!();
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_population_size_18_iterations() {
        let initial_population = "3,4,3,1,2".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 18;
        let result = brute_force_population_size(initial_population, iterations);
        assert_eq!(result, 26);
    }

    #[test]
    fn test_population_size_80_iterations() {
        let initial_population = "3,4,3,1,2".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 80;
        let result = brute_force_population_size(initial_population, iterations);
        assert_eq!(result, 5934);
    }

    #[test]
    fn test_population_size_18_iterations_queued_population_calculation() {
        let initial_population = "3,4,3,1,2".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 18;
        let result = queued_population_calculation(initial_population, iterations);
        assert_eq!(result, 26);
    }

    #[test]
    fn test_population_size_80_iterations_queued_population_calculation() {
        let initial_population = "3,4,3,1,2".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 80;
        let result = queued_population_calculation(initial_population, iterations);
        assert_eq!(result, 5934);
    }
}
