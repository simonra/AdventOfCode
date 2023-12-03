import System.IO
import Control.Monad

main = do
    contents <- readFile "sample_input.txt"
    print contents
