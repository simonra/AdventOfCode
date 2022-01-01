fn main() {
    let input = read_input_from_file("./src/input.txt");
    let result = calculate_power_consumption(&input);
    println!("Result was:");
    println!("{}", result);
}

fn read_input_from_file(filename: &str) -> Vec<String> {
    let content = std::fs::read_to_string(filename)
        .expect("Failed to read from file");
    return content.lines().map(str::to_string).collect();
}


fn calculate_power_consumption(report: &Vec<String>) -> u64 {

    let size_of_report_line = report[0].chars().count();
    let number_of_report_lines: usize = report.len().try_into().unwrap();

    let mut count_of_zeroes_per_column = std::iter::repeat(0)
        .take(size_of_report_line)
        .collect::<Vec<u16>>();

    let mut count_of_ones_per_column = std::iter::repeat(0)
        .take(size_of_report_line)
        .collect::<Vec<u16>>();

    for line_number in 0..number_of_report_lines {
        // let chars = report[line_number].chars();
        // if chars.nth(character_number) == Some('0') {...}
        let binary_encoded_chars = report[line_number].as_bytes();
        for character_number in 0..size_of_report_line {
            if binary_encoded_chars[character_number] == b'0' {
                count_of_zeroes_per_column[character_number] += 1;
            }
            else {
                count_of_ones_per_column[character_number] += 1;
            }
        }
    }

    let mut gamma_rate_string = String::with_capacity(size_of_report_line);
    let mut epsilon_rate_string = String::with_capacity(size_of_report_line);
    for i in 0..size_of_report_line {
        if count_of_zeroes_per_column[i] < count_of_ones_per_column[i] {
            gamma_rate_string.push('1');
            epsilon_rate_string.push('0');
        }
        else {
            gamma_rate_string.push('0');
            epsilon_rate_string.push('1');
        }
    }

    let gamma_rate_decimal = u64::from_str_radix(&gamma_rate_string, 2).unwrap();
    let epsilon_rate_decimal = u64::from_str_radix(&epsilon_rate_string, 2).unwrap();

    let power_consumption = gamma_rate_decimal * epsilon_rate_decimal;

    return power_consumption;
}

fn calculate_life_support_rating(report: &Vec<String>) -> u64 {
    unimplemented!();
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_calculate_power_consumption() {
        let input: Vec<String> =
            vec![
                "00100",
                "11110",
                "10110",
                "10111",
                "10101",
                "01111",
                "00111",
                "11100",
                "10000",
                "11001",
                "00010",
                "01010",
            ].iter().map(|&s| s.into()).collect();
        // let input = string_values.iter().map(str::to_string).collect();
        let result = calculate_power_consumption(&input);
        assert_eq!(result, 198);
    }

    #[test]
    fn test_calculate_life_support_rating() {
        let input: Vec<String> =
            vec![
                "00100",
                "11110",
                "10110",
                "10111",
                "10101",
                "01111",
                "00111",
                "11100",
                "10000",
                "11001",
                "00010",
                "01010",
            ].iter().map(|&s| s.into()).collect();
        // let input = string_values.iter().map(str::to_string).collect();
        let result = calculate_power_consumption(&input);
        assert_eq!(result, 230);
    }
}
