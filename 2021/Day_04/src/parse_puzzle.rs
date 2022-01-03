use crate::data_types::board_entry::*;
use crate::data_types::board::*;
use crate::data_types::drawn_number::*;
use crate::data_types::board_id::*;

pub fn input(input: &str) -> (Vec<Board>, Vec<DrawnNumber> ) {
    let input_grouped: Vec<&str> = input.split("\n\n").collect();

    let drawn_numbers = drawn_numbers(input_grouped[0]);

    let board_strings = input_grouped.iter().skip(1);

    let mut boards: Vec<Board> = Vec::new();

    board_strings
        .clone()
        .enumerate()
        .for_each(|board_enumeration| {
            let board_id = BoardId { value: board_enumeration.0 as u8 };
            let lines_as_text: Vec<&str> = board_enumeration.1.lines().collect();
            let parsed_lines: Vec<Vec<BoardEntry>> = lines_as_text
                .iter()
                .map(|line| -> Vec<BoardEntry> { return board_line(line) } )
                .collect();
            let number_of_lines_in_board = parsed_lines.len();
            let number_of_columns_in_board = parsed_lines[0].len();

            let board = Board {
                board_id: board_id,
                size_x: number_of_columns_in_board as u8,
                size_y: number_of_lines_in_board as u8,
                entries: parsed_lines.clone(),
                is_won: false,
                score: 0,
            };

            boards.push(board);
        }
    );

    return (boards, drawn_numbers);
}

fn drawn_numbers(input: &str) -> Vec<DrawnNumber> {
    return input
        .split(',')
        .map(|s| -> DrawnNumber {
            return DrawnNumber { value: s.parse().expect("Failed to parse number") }
        } )
        .collect();
}

fn board_line(input: &str) -> Vec<BoardEntry> {
    return input
        /*.replace("  ", " ")
        .split(" ")*/
        .split_whitespace()
        .enumerate()
        .map(|enumeration| -> BoardEntry {
            return BoardEntry {
                value: enumeration.1.parse().unwrap(),
                marked: false,
            }
        })
        .collect();
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
        // let result = input(SAMPLE_INPUT);

        // let entries = result.0;
        // let boards = result.1;
        // let input_numbers = result.2;

        let (boards, input_numbers) = input(SAMPLE_INPUT);

        assert_eq!(input_numbers[0].value, 7);
        assert_eq!(input_numbers[26].value, 1);

        assert_eq!(boards[0].size_x, 5);
        assert_eq!(boards[0].size_y, 5);

        // assert_eq!(entries.len(), 5 * 5 * 3);
    }

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
        let result = board_line(input);

        assert_eq!(result[0].value, 3);
        assert_eq!(result[0].marked, false);

        assert_eq!(result[3].value, 2);
        assert_eq!(result[3].marked, false);

        assert_eq!(result[4].value, 22);
        assert_eq!(result[4].marked, false);
    }
}
