use std::collections::HashMap;
use crate::data_types::*;

fn main() {
    let input_content = std::fs::read_to_string("./src/input.txt").expect("Failed to read from file");
    let overlap_basic = number_of_overlapping_horizontal_and_vertical_lines(&input_content);
    println!("Number of overlapping horizontal and vertical lintes:");
    println!("{}",overlap_basic);
}

fn number_of_overlapping_lines_with_diagonals(input: &str) -> u64 {
    let mut number_of_times_point_is_intersected: HashMap<Point, u16> = HashMap::new();

    parse_line_segments(input)
        .iter()
        .flat_map(|line| -> Vec<Point> {get_points_on_line_segment(*line)})
        .for_each(|point| {
            *number_of_times_point_is_intersected.entry(point).or_insert(0) += 1
        })
        ;

    return number_of_times_point_is_intersected
        .values()
        .filter(|value| **value > 1)
        .count()
        .try_into()
        .unwrap();
}

fn number_of_overlapping_horizontal_and_vertical_lines(input: &str) -> u64 {
    // let mut intersected_points: HashMap<Point, u16> = HashMap::new();
    let mut number_of_times_point_is_intersected: HashMap<Point, u16> = HashMap::new();
    // parse_line_segments(input).foreach
    // let parsed_lines = parse_line_segments(input);
    // let lines = parsed_lines.iter().filter(|line| line_segment_is_horizontal_or_vertical(**line));
    // lines.for_each(|line| {
    //     let points = get_points_on_line_segment(*line);

    // });

    parse_line_segments(input)
        .iter()
        .filter(|line| line_segment_is_horizontal_or_vertical(**line))
        .flat_map(|line| -> Vec<Point> {get_points_on_line_segment(*line)})
        .for_each(|point| {
            *number_of_times_point_is_intersected.entry(point).or_insert(0) += 1
        })
        ;

    return number_of_times_point_is_intersected
        .values()
        .filter(|value| **value > 1)
        .count()
        .try_into()
        .unwrap();
}

fn parse_line_segments(input: &str) -> Vec<LineSegment> {
    return input.lines().map(|s| LineSegment::parse(s)).collect();
}

fn line_segment_is_horizontal_or_vertical(line_segment: LineSegment) -> bool {
    return line_segment.beginning.x == line_segment.end.x || line_segment.beginning.y == line_segment.end.y;
}

fn get_points_on_line_segment(line_segment: LineSegment) -> Vec<Point> {
    let xs = get_values_between_inclusive(line_segment.beginning.x, line_segment.end.x);
    let ys = get_values_between_inclusive(line_segment.beginning.y, line_segment.end.y);
    let mut result: Vec<Point> = Vec::new();
    if xs.len() == 1 {
        for y in ys {
            result.push(Point {x: xs[0], y: y});
        }
    }
    else if ys.len() == 1 {
        for x in xs {
            result.push(Point {x: x, y: ys[0]});
        }
    }
    else if xs.len() == ys.len() {
        xs.iter().zip(ys).for_each(|(x, y)| result.push(Point {x: *x, y: y}));
    }
    else {
        panic!("Dealing with non-perfectly angled line segments is out of scope for this.");
    }
    return result;
}

fn get_values_between_inclusive<T: std::ops::Add<Output=T> + std::ops::Sub<Output=T> + Ord + Copy /*+ std::ops::Div<Output = T>*/ + From<u8>>(a: T, b: T) -> Vec<T> {
    if a == b {
        return vec![a];
    }

    let mut result: Vec<T> = Vec::new();

    let start = if a < b {a} else {b};
    let end = if a < b {b} else {a};

    let mut counter = T::from(0);
    let one = T::from(1);

    while start + counter <= end {
        result.push(start + counter);
        counter = counter + one;
    }

    if b < a {
        result.reverse();
    }
    return result;
}

mod data_types {
    #[derive(Debug, Copy, Clone)]
    pub struct LineSegment {
        pub beginning: Point,
        pub end: Point,
    }

    impl LineSegment {
        pub fn parse(input: &str) -> LineSegment {
            let default_point_value = Point {x: 0, y: 0};
            let mut line_segment = LineSegment { beginning: default_point_value, end: default_point_value };
            let mut points = input.split(" -> ").map(|s| Point::parse(s));
            line_segment.beginning = points.next().unwrap();
            line_segment.end = points.next().unwrap();
            return line_segment;
        }
    }

    #[derive(Debug, Copy, Clone, Eq, PartialEq, Hash)]
    pub struct Point {
        pub x: u16,
        pub y: u16,
    }

    impl Point {
        fn parse(input: &str) -> Point {
            // let split = input.split(',');
            // if split.clone().count() != 2 {
            //     panic!("Cannot parse point without exactly 2 coordinates.");
            // }
            return input.split(',').map(|s| -> u16 {s.parse().unwrap()}).collect::<Point>();
            // let numbers: &[u16; 2] = split.map(|s| s.parse()).collect();
            // return Point {x: numbers[0], y: ,}
            // unimplemented!();
        }
    }

    impl FromIterator<u16> for Point {
        fn from_iter<I: IntoIterator<Item=u16>>(iter: I) -> Self {
            // let mut c = MyCollection::new();
            // let mut p = Point { x: iter.next().unwrap(), y: iter.next().unwrap() };
            let mut p = Point { x: 0, y: 0 };
            let mut iterator = iter.into_iter();
            p.x = iterator.next().unwrap();
            p.y = iterator.next().unwrap();
            return p;
        }
    }

    #[cfg(test)]
    mod tests {
        use super::*;

        #[test]
        fn test_parse_point() {
            let result = Point::parse("0,0");
            assert_eq!(result.x, 0);
            assert_eq!(result.y, 0);

            let result_2 = Point::parse("100,20");
            assert_eq!(result_2.x, 100);
            assert_eq!(result_2.y, 20);
        }

        #[test]
        fn test_parse_line_segment() {
            let result = LineSegment::parse("9,4 -> 3,4");

            assert_eq!(result.beginning.x,9);
            assert_eq!(result.beginning.y,4);

            assert_eq!(result.end.x,3);
            assert_eq!(result.end.y,4);
        }
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    static SAMPLE_INPUT: &str =
        "0,9 -> 5,9\n\
        8,0 -> 0,8\n\
        9,4 -> 3,4\n\
        2,2 -> 2,1\n\
        7,0 -> 7,4\n\
        6,4 -> 2,0\n\
        0,9 -> 2,9\n\
        3,4 -> 1,4\n\
        0,0 -> 8,8\n\
        5,5 -> 8,2\n";

    #[test]
    fn test_number_of_overlapping_horizontal_and_vertical_lines() {
        let result = number_of_overlapping_horizontal_and_vertical_lines(SAMPLE_INPUT);
        assert_eq!(result, 5);
    }

    #[test]
    fn test_number_of_overlapping_lines_with_diagonals() {
        let result = number_of_overlapping_lines_with_diagonals(SAMPLE_INPUT);
        assert_eq!(result, 12);
    }

    #[test]
    fn test_parse_line_segments() {
        let result = parse_line_segments(SAMPLE_INPUT);

        println!("{:?}", result);

        assert_eq!(result.len(), 10);

        assert_eq!(result[0].beginning.x, 0);
        assert_eq!(result[0].beginning.y, 9);

        assert_eq!(result[9].end.x, 8);
        assert_eq!(result[9].end.y, 2);

        let expected_last_point = Point {x: 8, y: 2};
        assert_eq!(result.last().unwrap().end, expected_last_point)
    }

    #[test]
    fn test_line_segment_is_horizontal_or_vertical_gives_true_when_horizontal() {
        let input = LineSegment { beginning: Point { x: 0, y: 0, }, end: Point { x: 0, y: 1, },};
        let result = line_segment_is_horizontal_or_vertical(input);
        assert!(result);
    }

    #[test]
    fn test_line_segment_is_horizontal_or_vertical_gives_true_when_vertical() {
        let input = LineSegment { beginning: Point { x: 1, y: 1, }, end: Point { x: 0, y: 1, },};
        let result = line_segment_is_horizontal_or_vertical(input);
        assert!(result);
    }

    #[test]
    fn test_test_line_segment_is_horizontal_or_vertical_false_when_diagonal() {
        let input = LineSegment { beginning: Point { x: 0, y: 0, }, end: Point { x: 1, y: 1, },};
        let result = line_segment_is_horizontal_or_vertical(input);
        assert!(!result);
    }

    #[test]
    fn test_get_points_on_line_segment_horizontal() {
        let input = LineSegment { beginning: Point { x: 0, y: 0, }, end: Point { x: 0, y: 2, },};
        let result = get_points_on_line_segment(input);
        assert_eq!(result.len(), 3);
        assert!(result.contains(&Point { x: 0, y: 0, }));
        assert!(result.contains(&Point { x: 0, y: 1, }));
        assert!(result.contains(&Point { x: 0, y: 2, }));
    }

    #[test]
    fn test_get_points_on_line_segment_vertical() {
        let input = LineSegment { beginning: Point { x: 0, y: 0, }, end: Point { x: 2, y: 0, },};
        let result = get_points_on_line_segment(input);
        assert_eq!(result.len(), 3);
        assert!(result.contains(&Point { x: 0, y: 0, }));
        assert!(result.contains(&Point { x: 1, y: 0, }));
        assert!(result.contains(&Point { x: 2, y: 0, }));
    }

    #[test]
    fn test_get_points_on_line_segment_diagonal_down_right() {
        let input = LineSegment { beginning: Point { x: 0, y: 0, }, end: Point { x: 2, y: 2, },};
        let result = get_points_on_line_segment(input);
        assert_eq!(result.len(), 3);
        assert!(result.contains(&Point { x: 0, y: 0, }));
        assert!(result.contains(&Point { x: 1, y: 1, }));
        assert!(result.contains(&Point { x: 2, y: 2, }));
    }

    #[test]
    fn test_get_points_on_line_segment_diagonal_up_right() {
        let input = LineSegment { beginning: Point { x: 0, y: 2, }, end: Point { x: 2, y: 0, },};
        let result = get_points_on_line_segment(input);
        assert_eq!(result.len(), 3);
        assert!(result.contains(&Point { x: 0, y: 2, }));
        assert!(result.contains(&Point { x: 1, y: 1, }));
        assert!(result.contains(&Point { x: 2, y: 0, }));
    }

    #[test]
    fn test_get_points_on_line_segment_diagonal_down_left() {
        let input = LineSegment { beginning: Point { x: 2, y: 0, }, end: Point { x: 0, y: 2, },};
        let result = get_points_on_line_segment(input);
        assert_eq!(result.len(), 3);
        assert!(result.contains(&Point { x: 2, y: 0, }));
        assert!(result.contains(&Point { x: 1, y: 1, }));
        assert!(result.contains(&Point { x: 0, y: 2, }));
    }

    #[test]
    fn test_get_points_on_line_segment_diagonal_up_left() {
        let input = LineSegment { beginning: Point { x: 2, y: 2, }, end: Point { x: 0, y: 0, },};
        let result = get_points_on_line_segment(input);
        assert_eq!(result.len(), 3);
        assert!(result.contains(&Point { x: 2, y: 2, }));
        assert!(result.contains(&Point { x: 1, y: 1, }));
        assert!(result.contains(&Point { x: 0, y: 0, }));
    }

    #[test]
    fn test_get_points_on_line_segment_backwards() {
        let input = LineSegment { beginning: Point { x: 0, y: 1, }, end: Point { x: 0, y: 0, },};
        let result = get_points_on_line_segment(input);
        assert_eq!(result.len(), 2);
        assert!(result.contains(&Point { x: 0, y: 0, }));
        assert!(result.contains(&Point { x: 0, y: 1, }));
    }

    #[test]
    fn test_get_values_between_inclusive() {
        let result: Vec<u64> = get_values_between_inclusive(0, 10);

        assert_eq!(result.len(), 11);
        for (index, value) in result.iter().enumerate() {
            assert_eq!(index as u64, *value);
        }

        let result_2: Vec<u64> = get_values_between_inclusive(5, 0);

        assert_eq!(result_2.len(), 6);
        for (index, value) in result_2.iter().enumerate() {
            assert_eq!(5 - index as u64, *value);
        }
    }

    #[test]
    fn test_insert_into_hashmap() {
        let mut map: HashMap<Point, u16> = HashMap::new();
        let key_point = Point {x: 0, y: 0};

        *map.entry(key_point).or_insert(0) += 1;
        assert_eq!(map[&key_point], 1);

        *map.entry(key_point).or_insert(0) += 1;
        assert_eq!(map[&key_point], 2);
    }
}
