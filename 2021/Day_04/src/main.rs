mod data_types;
use crate::data_types::board::*;
use crate::data_types::drawn_number::*;

mod parse_puzzle;

fn main() {
    println!("Hello, world!");
    let input_content = std::fs::read_to_string("./src/input.txt").expect("Failed to read from file");
    let (boards, input_numbers) = parse_puzzle::input(&input_content);
    let winning_score = calculate_winning_score(boards, input_numbers);
    println!("Winning score is:");
    println!("{}", winning_score);
}

fn calculate_winning_score(boards: Vec<Board>, drawn_numbers: Vec<DrawnNumber>) -> u64 {
    let mut boards = boards.to_owned();

    for drawn_number in drawn_numbers {

        // println!("{:?}", drawn_number);

        for board in boards.iter_mut() {
            for row_of_entries in board.entries.iter_mut() {
                for entry in row_of_entries {
                    if entry.value == drawn_number.value {
                        entry.marked = true;
                    }
                }
            }

            let board_won = check_if_board_is_won(&board);
            if board_won {
                // println!("{:?}", board);
                return calculate_score(&board, drawn_number);
            }
        }
    }

    panic!("No boards were won. This is an unacceptable state of affairs.");
}

fn calculate_score(board: &Board, winning_number: DrawnNumber) -> u64 {
    let sum = calculate_sum_of_unmarked_entries(board);
    let product = sum * (winning_number.value as u64);
    return product;
}

fn calculate_sum_of_unmarked_entries(board: &Board) -> u64 {
    let mut sum: u64 = 0;
    for row_number in 0..board.size_x.into() {
        for column_number in 0..board.size_y.into() {
            if !board.entries[row_number][column_number].marked {
                sum += board.entries[row_number][column_number].value as u64;
            }
        }
    }

    return sum;
}

fn check_if_board_is_won(board: &Board) -> bool {
    let mut count_of_marked_entries_per_row = std::iter::repeat(0)
        .take(board.size_x.into())
        .collect::<Vec<u8>>();

    let mut count_of_marked_entries_per_column = std::iter::repeat(0)
        .take(board.size_y.into())
        .collect::<Vec<u8>>();

    for row_number in 0..board.size_x.into() {
        for column_number in 0..board.size_y.into() {
            if board.entries[row_number][column_number].marked {
                count_of_marked_entries_per_row[row_number] += 1;
                count_of_marked_entries_per_column[column_number] += 1;
            }
        }
    }

    for row_sum_checked in count_of_marked_entries_per_row {
        if row_sum_checked == board.size_x {
            return true;
        }
    }

    for column_sum_checked in count_of_marked_entries_per_column {
        if column_sum_checked == board.size_y {
            return true;
        }
    }

    return false;
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
    fn test_calculate_winning_score() {
        let (boards, input_numbers) = parse_puzzle::input(SAMPLE_INPUT);
        let result = calculate_winning_score(boards, input_numbers);
        assert_eq!(result, 4512);
    }

    #[test]
    fn test_check_if_board_is_won() {
        let (boards, _input_numbers) = parse_puzzle::input(SAMPLE_INPUT);
        let boards = boards.to_owned();
        let mut chosen_board = boards[0].clone();

        let is_won_before_played = check_if_board_is_won(&chosen_board);
        assert_eq!(is_won_before_played, false);

        for row_of_entries in chosen_board.entries.iter_mut(){
            for entry in row_of_entries {
                entry.marked = true;
            }
        }

        let is_won_after_played = check_if_board_is_won(&chosen_board);
        assert_eq!(is_won_after_played, true);
    }

    #[test]
    fn test_calculate_sum_of_unmarked_entries() {
        let (boards, _input_numbers) = parse_puzzle::input(SAMPLE_INPUT);
        let boards = boards.to_owned();
        let mut chosen_board = boards[0].clone();

        let sum_all_unmarked = calculate_sum_of_unmarked_entries(&chosen_board);
        assert_eq!(sum_all_unmarked,
22+13+17+11+0+
8+2+23+4+24+
21+9+14+16+7+
6+10+3+18+5+
1+12+20+15+19
        );

        for row_of_entries in chosen_board.entries.iter_mut(){
            for entry in row_of_entries {
                entry.marked = true;
            }
        }

        let sum_all_marked = calculate_sum_of_unmarked_entries(&chosen_board);
        assert_eq!(sum_all_marked, 0);
    }

    #[test]
    fn test_calculate_score() {
        let (boards, _input_numbers) = parse_puzzle::input(SAMPLE_INPUT);
        let boards = boards.to_owned();
        let mut chosen_board = boards[2].clone();

        let marked_numbers: Vec<u8> = vec![14,21,17,24,4,9,23,11,5,2,0,7];

        for row_of_entries in chosen_board.entries.iter_mut(){
            for entry in row_of_entries {
                if marked_numbers.contains(&entry.value){
                    entry.marked = true;
                }
            }
        }

        let is_won = check_if_board_is_won(&chosen_board);
        assert_eq!(is_won, true);

        let sum_all_unmarked = calculate_sum_of_unmarked_entries(&chosen_board);
        assert_eq!(sum_all_unmarked,
0+0+0+0+0+
10+16+15+0+19+
18+8+0+26+20+
22+0+13+6+0+
0+0+12+3+0
        );

        let winning_number = DrawnNumber { value: 24 };
        let winning_score = calculate_score(&chosen_board, winning_number);
        assert_eq!(winning_score, 4512);
    }
}
