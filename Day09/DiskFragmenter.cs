using AdventOfCode2024.Day05;

namespace AdventOfCode2024.Day09
{
    internal class DiskFragmenter : SolutionBase
    {
        public override string Name => "Day 09: Disk Fragmenter";

        public override int Day => 9;

        public override string Solution(Part part, string input)
        {
            var reader = new ImageReader(input.ToCharArray());
            
            var head = reader.ReadImage();
            
            Console.WriteLine(reader.PrintImage(head));

            var top = head.FindRoot();

            top.Defragment(head);
        }
    }

    internal class ImageReader(char[] image)
    {
        private char[] Image { get; init; } = image;

        public Block ReadImage()
        {
            Block head = new Space();
            var (initialFileSize, initialPadding) = (Image[0] - '0', Image[1] - '0');
            if (initialFileSize != 0)
            {
                head = new FileSection(0);
                for (int i = 1; i < initialFileSize; i++)
                {
                    var current = head.Tail ?? new FileSection(0, head);
                    head.Tail = current;
                    head = head.Tail;
                }
                for (int j = 0; j < initialPadding; j++)
                {
                    head.Tail = new Space(head);
                    head = head.Tail;
                }
            }
            else
            {
                for (int i = 1; i < initialPadding; i++)
                {
                    head.Tail = new Space();
                    head = head.Tail;
                }
            }

            var fileId = 1;
            for (int i = 2; i < Image.Length; i += 2)
            {
                var fileSize = Image[i] - '0';
                for (int f = 0; f < fileSize; f++)
                {
                    head.Tail = new FileSection(fileId, head);
                    head = head.Tail;
                }
                if (fileSize > 0) fileId++;

                if (i == Image.Length - 1) break;

                var padding = Image[i + 1] - '0';
                for (int p = 0; p < padding; p++)
                {
                    head.Tail = new Space(head);
                    head = head.Tail;
                }
            }

            return head;
        }

        public string PrintImage(Block end)
        {
            var image = string.Empty;

            while (end != null)
            {
                image += end is FileSection file ? file.Id: ".";
                end = end.Head;
            }

            return new string(image.Reverse().ToArray());
        }
    }

    internal class ImageWriter(Block head)
    {
        public Block Head { get; init; } = head;

        public string WriteImage()
        {
            return "";
        }
    }

    /// <summary>
    /// File system block; a certain amount of memory with a contiguous head and tail section
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    internal record Block(Block? head = null, Block? tail = null)
    {
        /// <summary>
        /// The block BEFORE this block in the filesystem
        /// </summary>
        public Block? Head { get; set; } = head;
        /// <summary>
        /// The block AFTER this block in the filesystem
        /// </summary>
        public Block? Tail { get; set; } = tail;

        public Block FindRoot()
        {
            var current = this;

            while (Head != null) current = Head;

            return current;
        }

        internal void Defragment(Block end)
        {
            while (Tail is not null && end is not null)
            {
                var current = this;

                // Find Next Space
                while (current.Tail is not null && current is not Space)
                    current = current.Tail;

                // swap space and end
                (current, end) =
                    (
                        end with { Head = current.Head, Tail = current.Tail },
                        current with { Head = end.Head, Tail = end.Tail }
                    );
            }
        }
    }

    /// <summary>
    /// Contains Data; has an ID marking its file
    /// </summary>
    /// <param name="id">ID of filesection, ID shared between files</param>
    internal record FileSection(int id, Block? head = null, Block? tail = null) : Block(head, tail)
    {
        /// <summary>
        /// The ID of the file this section belongs to
        /// </summary>
        public int Id { get; init; } = id;
    }

    /// <summary>
    /// Empty block of memory
    /// </summary>
    internal record Space(Block? head = null, Block? tail = null) : Block(head, tail);
}
