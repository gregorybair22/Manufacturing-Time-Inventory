# How to Convert USAGE_GUIDE.md to PDF

## Option 1: Using Online Converters (Easiest)

1. Open `USAGE_GUIDE.md` in a text editor
2. Copy all content
3. Use one of these online converters:
   - **Markdown to PDF**: https://www.markdowntopdf.com/
   - **Dillinger**: https://dillinger.io/ (Export as PDF)
   - **StackEdit**: https://stackedit.io/ (Export as PDF)

## Option 2: Using VS Code Extension

1. Install "Markdown PDF" extension in VS Code
2. Open `USAGE_GUIDE.md`
3. Right-click → "Markdown PDF: Export (pdf)"

## Option 3: Using Pandoc (Command Line)

1. Install Pandoc: https://pandoc.org/installing.html
2. Install a PDF engine (MiKTeX or TeX Live)
3. Run:
   ```bash
   pandoc USAGE_GUIDE.md -o USAGE_GUIDE.pdf --pdf-engine=xelatex
   ```

## Option 4: Using GitHub/GitLab

1. Push the file to a GitHub/GitLab repository
2. View the file on GitHub/GitLab
3. Use browser's Print → Save as PDF

## Option 5: Using Chrome/Edge Browser

1. Open `USAGE_GUIDE.md` in VS Code
2. Use "Markdown Preview Enhanced" extension
3. Right-click preview → "Chrome (Puppeteer)" → "PDF"

## Recommended: Online Converter

The easiest method is Option 1 - use https://www.markdowntopdf.com/:
1. Upload `USAGE_GUIDE.md` file
2. Click "Convert"
3. Download the PDF
