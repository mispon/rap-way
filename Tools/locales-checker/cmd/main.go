package main

import (
	"encoding/json"
	"fmt"
	"io/fs"
	"log"
	"os"
	"strings"
)

const (
	filesPath  = "../../Assets/StreamingAssets/"
	targetFile = "ru.json"
)

var excludeSuffixes = []string{
	".meta",
	"google-services-desktop.json",
}

func main() {
	dir, err := os.ReadDir(filesPath)
	if err != nil {
		log.Fatal(err)
	}

	var i int
	for _, entry := range dir {
		var shouldSkip bool
		for _, suffix := range excludeSuffixes {
			if strings.HasSuffix(entry.Name(), suffix) {
				shouldSkip = true
				break
			}
		}

		if shouldSkip {
			continue
		}

		dir[i] = entry
		i++
	}

	compareFiles(dir[:i])
}

func compareFiles(dir []fs.DirEntry) {
	var target map[string]string

	var i int
	for _, entry := range dir {
		if fname := entry.Name(); fname == targetFile {
			target = readFile(fname)
			continue
		}

		dir[i] = entry
		i++
	}

	for _, entry := range dir[:i] {
		fmt.Printf("%s -- %s\n", targetFile, entry.Name())

		file := readFile(entry.Name())

		if eq := compareMaps(target, file); eq {
			fmt.Println("OK")
		} else {
			fmt.Println("FAIL")
		}
		fmt.Println()
	}
}

func compareMaps(a, b map[string]string) bool {
	res := true

	if len(a) != len(b) {
		fmt.Printf("\tdiff len (a: %d, b: %d)\n", len(a), len(b))
		res = false
	}

	// check a keys
	for k := range a {
		if _, ok := b[k]; !ok {
			fmt.Printf("\tkey %q is missing in second file\n", k)
			res = false
		}
	}

	// check b keys
	for k := range b {
		if _, ok := a[k]; !ok {
			fmt.Printf("\tkey %q is missing in origin file\n", k)
			res = false
		}
	}

	return res
}

type (
	locale struct {
		Items []localeItem
	}

	localeItem struct {
		Key   string
		Value string
	}
)

func readFile(fileName string) map[string]string {
	path := fmt.Sprintf("%s/%s", filesPath, fileName)

	content, err := os.ReadFile(path)
	if err != nil {
		log.Fatal(err)
	}

	var loc locale
	if err = json.Unmarshal(content, &loc); err != nil {
		log.Fatal(err)
	}

	res := make(map[string]string, len(loc.Items))
	for _, item := range loc.Items {
		res[item.Key] = item.Value
	}

	return res
}
