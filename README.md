# JazzNotes
### [swegrock.github.io/jazznotes/](https://swegrock.github.io/jazznotes/)
Transcribing and analysing music has never been easier...👌

## Summary
Transcribing and analysing is a mandatory for most jazz musicians. Learning and developing sheet music can be a chore and JazzNotes aims to simplify the process,
It allows jazz musicians to take notes from PDFs and images, then they can be annotated, tagged and stored for usage and future reference.

Here's a few of its main features:
- A simple viewer for looking through your sheet music.
- Search transcriptions and notes by tag to find exactly what you're looking for.
- Add titles, notes, tasks, images and tags to each note.
- View a complete task list so you know what's done and what's not.
- An image viewer for zooming into the finer details.
- Keeps a copy of your sheet music, so you don't need to worry about what happens to your PDFs and images.

## Building
JazzNotes requires .NET 5. Run *publish win/mac* depending on which platform you want to build for.

## Dev Notes
JazzNotes is a mini project made on my lonesome based on an idea dreamt up by my brother to improve his time when transcribing Jazz music. I decided to take up the project as an effort to test Avalonia UI and see if it works for me. It's not in depth and it was made very quickly.
I need to do some tidying up of the code and remove some bits/add some bits you know the drill. I implore anyone who enjoys doing that to give it a go. I'll likely make this main priority for the next update.

So my opinion of Avalonia is, that its not my cup of tea.
At first the benefits seem to far outweigh the negatives, it has most of WPFs features (some improvements also). But quickly things start to feel rigid, for example when experimenting with styling and unconventional UI design, the limitations become apparent and weird half-baked syntax becomes the norm.
I often found myself playing dirty and adding to the code behind more often than I'd like. This is an example of where more documentation would've helped a newbie like myself hugely, but as Avalonia isn't official and .NET 6 is coming out with multiple cross-platform UI frameworks to pick from, its unlikely this project will get much coverage.
It's safe to say I love what Avalonia is and what it represents, and the development group behind it are an unbelievable bunch who deserve far more credit for what they've accomplished.
2 cents over.

Working with PDFs was a rewarding experience, especially when I finally managed to get GhostScript to work the way I wanted it to.
I initially wanted to avoid external libraries like GhostScript and there were some really promising frameworks available [like this](https://pdfclown.org), but unfortunately they just didn't do the job. Handling PDFs exported from Sibelius were literally coming out blank due the way the content is displayed. So testing out GhostScript and seeing that it worked out of the box, it became the defacto solution.

When it came to file handling, I didn't want the program to store images permanently on the computer in an attempt to keep things lightweight. The solution was to store *snips* which were 2 rect structs stored in the XML document, and then each time a transcription was loaded the snips would be used to create the sub-images of the main transcription image. This can be a hefty task depending on how many notes have been made, but it works to keep things lightweight. Added images (via the notes) are stored in the JazzNotes folder as copies so that the originals can be moved, deleted etc.
Providing an open ended system like JazzNotes for an audience who are not often familiar with the file system was eye-opening task. I often found myself debating/discussing the systems with my brother while he tested out the solutions. Overall there are a few known issues with file handling, but hopefully most have been ironed out.

## License:
[AGPLv3](https://www.gnu.org/licenses/agpl-3.0.en.html)
