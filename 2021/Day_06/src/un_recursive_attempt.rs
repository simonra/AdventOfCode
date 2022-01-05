pub fn brute_force_population_size(initial_population: Vec<u8>, iterations: u8) -> u64 {
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
    }

    return population.len().try_into().unwrap();
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
}
