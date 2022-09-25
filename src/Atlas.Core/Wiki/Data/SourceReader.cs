// call api query all pages with max limit, then call api parse for each page. Delay of 0.1 ms per page
// should run in parallel with the parser with a concurrentqueue intermediary