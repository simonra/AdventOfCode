fn main() {
    println!("Hello, world!");
}

fn number_of_overlapping_horizontal_and_vertical_lines(input: &str) -> u64 {
    unimplemented!();
}

mod data_types {
    #[derive(Debug, Copy, Clone)]
    pub struct LineSegment {
        pub beginning: Point,
        pub end: Point,
    }

    impl LineSegment {
        fn parse(input: &str) -> LineSegment {
            let split = input.split(" -> ");
            let default_point_value = Point {x: 0, y: 0};
            let mut line_segment = LineSegment { beginning: default_point_value, end: default_point_value };
            let mut points = split.map(|s| Point::parse(s));
            line_segment.beginning = points.next().unwrap();
            line_segment.end = points.next().unwrap();
            return line_segment;
        }
    }

    #[derive(Debug, Copy, Clone)]
    pub struct Point {
        pub x: u16,
        pub y: u16,
    }

    impl Point {
        fn parse(input: &str) -> Point {
            let split = input.split(',');
            // if split.clone().count() != 2 {
            //     panic!("Cannot parse point without exactly 2 coordinates.");
            // }
            return split.map(|s| -> u16 {s.parse().unwrap()}).collect::<Point>();
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
        "0,9 -> 5,9\
        8,0 -> 0,8\
        9,4 -> 3,4\
        2,2 -> 2,1\
        7,0 -> 7,4\
        6,4 -> 2,0\
        0,9 -> 2,9\
        3,4 -> 1,4\
        0,0 -> 8,8\
        5,5 -> 8,2";

    #[test]
    fn test_calculate_winning_score() {
        let result = number_of_overlapping_horizontal_and_vertical_lines(SAMPLE_INPUT);
        assert_eq!(result, 5);
    }


}
