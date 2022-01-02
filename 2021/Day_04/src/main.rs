mod data_types;
use crate::data_types::board_entry::*;
use crate::data_types::board::*;
use crate::data_types::drawn_number::*;

fn main() {
    println!("Hello, world!");
}

// fn parse_input(&str) ->

mod parse_puzzle {
    use crate::data_types::board_entry::*;
    use crate::data_types::board::*;
    use crate::data_types::drawn_number::*;
    use crate::data_types::board_id::*;

    pub fn input(input: &str) -> (&Vec<BoardEntry>, &Vec<Board>, &Vec<DrawnNumber> ) {
        unimplemented!();
    }

    fn drawn_numbers(input: &str) -> Vec<DrawnNumber> {
        return input
            .split(',')
            .map(|s| -> DrawnNumber {
                return DrawnNumber { value: s.parse().expect("Failed to parse number") }
            } )
            .collect();
    }

    fn board_line(input: &str, board_id: BoardId, row_number: u8 ) -> Vec<BoardEntry> {
        return input
            /*.replace("  ", " ")
            .split(" ")*/
            .split_whitespace()
            .enumerate()
            .map(|enumeration| -> BoardEntry {
                return BoardEntry {
                    board_id: board_id,
                    x: enumeration.0 as u8,
                    y: row_number,
                    value: enumeration.1.parse().unwrap(),
                    marked: false,
                }
            })
            .collect();
    }

    #[cfg(test)]
    mod tests {
        use super::*;
        #[test]
        fn test_parse_drawn_numbers() {
            let input = "1,2,3,4";
            let result = drawn_numbers(input);

            assert_eq!(result[0].value, 1);
            assert_eq!(result[1].value, 2);
            assert_eq!(result[2].value, 3);
            assert_eq!(result[3].value, 4);
        }

        #[test]
        fn test_parse_board_line() {
            let input = " 3 15  0  2 22";
            let board_id_value = 3;
            let y = 2;
            let result = board_line(input, BoardId { value: board_id_value}, y);

            assert_eq!(result[0].board_id.value, board_id_value);
            assert_eq!(result[0].x, 0);
            assert_eq!(result[0].y, y);
            assert_eq!(result[0].value, 3);
            assert_eq!(result[0].marked, false);

            assert_eq!(result[3].board_id.value, board_id_value);
            assert_eq!(result[3].x, 3);
            assert_eq!(result[3].y, y);
            assert_eq!(result[3].value, 2);
            assert_eq!(result[3].marked, false);

            assert_eq!(result[4].board_id.value, board_id_value);
            assert_eq!(result[4].x, 4);
            assert_eq!(result[4].y, y);
            assert_eq!(result[4].value, 22);
            assert_eq!(result[4].marked, false);
        }
    }
}

fn calculate_winning_score(/*ToDo*/) -> u64 {
    unimplemented!();
}

#[cfg(test)]
mod tests {
    use super::*;

    static SAMPLE_INPUT: &str =
r"7,4,9,5,11,17,23,2,0,14,21,24,10,16,13,6,15,25,12,22,18,20,8,19,3,26,1

22 13 17 11  0
 8  2 23  4 24
21  9 14 16  7
 6 10  3 18  5
 1 12 20 15 19

 3 15  0  2 22
 9 18 13 17  5
19  8  7 25 23
20 11 10 24  4
14 21 16 12  6

14 21 17 24  4
10 16 15  9 19
18  8 23 26 20
22 11 13  6  5
 2  0 12  3  7
";

    #[test]
    fn test_parse_puzzle_input() {
        let result = parse_puzzle::input(SAMPLE_INPUT);
        unimplemented!();
    }
}
