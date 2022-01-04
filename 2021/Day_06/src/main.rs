fn main() {
    println!("Hello, world!");
}

fn population_size(initial_population: Vec<u8>, iterations: u8) -> u64 {
    unimplemented!();
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
    fn test_population_size_two_individual() {
        let initial_population = "0,0".split(',').map(|s| s.parse().unwrap()).collect();
        let iterations = 1;
        let result = population_size(initial_population, iterations);
        assert_eq!(result, 4);
    }
}
