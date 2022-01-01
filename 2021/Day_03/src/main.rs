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
    let report_as_binary_characters: Vec<&[u8]> = report.iter().map(|s| s.as_bytes()).collect();
    let item_size = report_as_binary_characters[0].len();
    let oxygen_generator_rating = calculate_oxygen_generator_rating(&report_as_binary_characters, item_size);
    let co2_scrubber_rating = calculate_co2_scrubber_rating(&report_as_binary_characters, item_size);
    let life_support_rating = oxygen_generator_rating * co2_scrubber_rating;
    return life_support_rating;
}

fn calculate_oxygen_generator_rating(report_items: &Vec<&[u8]>, item_size: usize) -> u64 {
    // return report_items.iter();
    // let mut oxygen_level_measurements = report_items.;
    let mut oxygen_level_measurements = report_items.to_vec();
    for column in 0..item_size {
        if oxygen_level_measurements.len() == 1 {
            break;
        }
        let mcb = find_most_common_bit_in_column(&oxygen_level_measurements, column, '1');
        if mcb == '1' {
            oxygen_level_measurements = oxygen_level_measurements.into_iter().filter(|item| item[column] == b'1').collect();
        }
        else {
            oxygen_level_measurements = oxygen_level_measurements.into_iter().filter(|item| item[column] == b'0').collect();
        }
    }
    if oxygen_level_measurements.len() != 1 {
        panic!("Expected to only have 1 oxygen reading left after filtering, now I don't know what to do :(");
    }

    let oxygen_level_measurements_string = String::from_utf8_lossy(oxygen_level_measurements[0]);

    return u64::from_str_radix(&oxygen_level_measurements_string, 2).unwrap();
}

fn calculate_co2_scrubber_rating(report_items: &Vec<&[u8]>, item_size: usize) -> u64 {
    // return report_items.iter();
    // let mut oxygen_level_measurements = report_items.;
    let mut oxygen_level_measurements = report_items.to_vec();
    for column in 0..item_size {
        if oxygen_level_measurements.len() == 1 {
            break;
        }
        let mcb = find_most_common_bit_in_column(&oxygen_level_measurements, column, '1');
        if mcb == '1' {
            oxygen_level_measurements = oxygen_level_measurements.into_iter().filter(|item| item[column] == b'0').collect();
        }
        else {
            oxygen_level_measurements = oxygen_level_measurements.into_iter().filter(|item| item[column] == b'1').collect();
        }
    }
    if oxygen_level_measurements.len() != 1 {
        panic!("Expected to only have 1 oxygen reading left after filtering, now I don't know what to do :(");
    }

    let oxygen_level_measurements_string = String::from_utf8_lossy(oxygen_level_measurements[0]);

    return u64::from_str_radix(&oxygen_level_measurements_string, 2).unwrap();
}

fn find_most_common_bit_in_column(report_items: &Vec<&[u8]>, column: usize, tiebreaker: char) -> char {
    let mut count_of_ones = 0;
    let mut count_of_zeroes = 0;
    for line in report_items {
        if line[column] == b'0' {
            count_of_zeroes += 1;
        }
        else {
            count_of_ones += 1;
        }
    }
    if count_of_zeroes < count_of_ones {
        return '1';
    }
    else if count_of_zeroes > count_of_ones {
        return '0'
    }

    return tiebreaker;
}

// fn notes() {
//     ones = reports.filter(|r| -> r[current] == 1);
//     zeroes = reports.filter(|r| -> r[current] == 0);
//     if(ones.len > zeroes)
//         msb = 1;
//     else if(zeroes > ones)
//         msb = 0;
//     else
//         msb = 1;
// }

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
        let result = calculate_life_support_rating(&input);
        assert_eq!(result, 230);
    }

    #[test]
    fn test_calculate_oxygen_generator_rating() {
        let input =
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
            ].iter().map(|&s| s.as_bytes()).collect::<Vec<&[u8]>>();
        let result = calculate_oxygen_generator_rating(&input, 5);
        assert_eq!(result, 23);
    }

    #[test]
    fn test_calculate_co2_scrubber_rating() {
        let input =
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
            ].iter().map(|&s| s.as_bytes()).collect::<Vec<&[u8]>>();
        let result = calculate_co2_scrubber_rating(&input, 5);
        assert_eq!(result, 10);
    }

    #[test]
    fn test_find_most_common_bit_in_column(){
        let input =
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
            ].iter().map(|&s| s.as_bytes()).collect::<Vec<&[u8]>>();
        let result = find_most_common_bit_in_column(&input, 0, '1');
        assert_eq!(result, '1');
    }

    #[test]
    fn test_find_most_common_bit_in_column_with_tie(){
        let input =
            vec![
                "10110",
                "10111",
            ].iter().map(|&s| s.as_bytes()).collect::<Vec<&[u8]>>();
        let result = find_most_common_bit_in_column(&input, 4, '1');
        assert_eq!(result, '1');
    }
}
