using CitationTool.Client.Models;

namespace CitationTool.Client.Services;

public class SeedDataService
{
    private readonly IStorageService _storage;
    private readonly IDomainService _domainService;
    private readonly ICitationService _citationService;

    public SeedDataService(IStorageService storage, IDomainService domainService, ICitationService citationService)
    {
        _storage = storage;
        _domainService = domainService;
        _citationService = citationService;
    }

    public async Task<bool> SeedIfEmptyAsync()
    {
        var existingCitations = await _storage.GetAllCitationsAsync();
        if (existingCitations.Count > 0)
            return false;

        var domains = CreateDomains();
        foreach (var domain in domains)
        {
            await _storage.SaveDomainAsync(domain);
        }

        var citations = CreateCitations(domains);
        await _storage.BulkAddCitationsAsync(citations);

        return true;
    }

    private List<Domain> CreateDomains()
    {
        return new List<Domain>
        {
            new() { Id = Guid.NewGuid(), Name = "Software Engineering", Description = "Software development methodologies, practices, and tools", Color = "#0d6efd" },
            new() { Id = Guid.NewGuid(), Name = "Artificial Intelligence", Description = "Machine learning, deep learning, and AI systems", Color = "#6f42c1" },
            new() { Id = Guid.NewGuid(), Name = "Security", Description = "Cybersecurity, cryptography, and secure systems", Color = "#dc3545" },
            new() { Id = Guid.NewGuid(), Name = "DevOps & Lifecycle", Description = "CI/CD, infrastructure, and software lifecycle management", Color = "#198754" },
            new() { Id = Guid.NewGuid(), Name = "LLM & Agents", Description = "Large language models, AI agents, and tool augmentation", Color = "#fd7e14" },
            new() { Id = Guid.NewGuid(), Name = "Distributed Systems", Description = "Cloud computing, distributed architecture, and scalability", Color = "#0dcaf0" },
            new() { Id = Guid.NewGuid(), Name = "Data Engineering", Description = "Data pipelines, databases, and data management", Color = "#20c997" },
            new() { Id = Guid.NewGuid(), Name = "Human-Computer Interaction", Description = "UX, accessibility, and human factors", Color = "#d63384" },
            new() { Id = Guid.NewGuid(), Name = "Cloud Computing", Description = "Cloud platforms, serverless, and cloud-native architecture", Color = "#ff6b6b" },
            new() { Id = Guid.NewGuid(), Name = "Networking", Description = "Network protocols, architecture, and infrastructure", Color = "#4ecdc4" },
            new() { Id = Guid.NewGuid(), Name = "Database Systems", Description = "Database design, query optimization, and storage engines", Color = "#45b7d1" },
            new() { Id = Guid.NewGuid(), Name = "Programming Languages", Description = "Language design, compilers, and type systems", Color = "#96ceb4" },
            new() { Id = Guid.NewGuid(), Name = "Testing & Quality", Description = "Software testing, quality assurance, and verification", Color = "#dda0dd" },
            new() { Id = Guid.NewGuid(), Name = "Web Development", Description = "Frontend, backend, and full-stack web technologies", Color = "#ffeaa7" }
        };
    }

    private List<Citation> CreateCitations(List<Domain> domains)
    {
        var softwareEng = domains.First(d => d.Name == "Software Engineering").Id;
        var ai = domains.First(d => d.Name == "Artificial Intelligence").Id;
        var security = domains.First(d => d.Name == "Security").Id;
        var devops = domains.First(d => d.Name == "DevOps & Lifecycle").Id;
        var llmAgents = domains.First(d => d.Name == "LLM & Agents").Id;
        var distributed = domains.First(d => d.Name == "Distributed Systems").Id;
        var dataEng = domains.First(d => d.Name == "Data Engineering").Id;
        var hci = domains.First(d => d.Name == "Human-Computer Interaction").Id;
        var cloud = domains.First(d => d.Name == "Cloud Computing").Id;
        var networking = domains.First(d => d.Name == "Networking").Id;
        var databases = domains.First(d => d.Name == "Database Systems").Id;
        var progLang = domains.First(d => d.Name == "Programming Languages").Id;
        var testing = domains.First(d => d.Name == "Testing & Quality").Id;
        var webDev = domains.First(d => d.Name == "Web Development").Id;

        var citations = new List<Citation>();

        // ==================== LLM & AGENTS ====================
        citations.AddRange(new[]
        {
            new Citation
            {
                Title = "Attention Is All You Need",
                Authors = new List<string> { "Ashish Vaswani", "Noam Shazeer", "Niki Parmar", "Jakob Uszkoreit", "Llion Jones", "Aidan N. Gomez", "Lukasz Kaiser", "Illia Polosukhin" },
                Type = CitationType.InProceedings,
                Year = 2017,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.1706.03762",
                Url = "https://arxiv.org/abs/1706.03762",
                Abstract = "The dominant sequence transduction models are based on complex recurrent or convolutional neural networks. We propose a new simple network architecture, the Transformer, based solely on attention mechanisms.",
                Tags = new List<string> { "transformer", "attention", "deep-learning", "seminal" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Language Models are Few-Shot Learners",
                Authors = new List<string> { "Tom Brown", "Benjamin Mann", "Nick Ryder", "Melanie Subbiah", "Jared Kaplan", "Prafulla Dhariwal", "Arvind Neelakantan", "Pranav Shyam", "Girish Sastry", "Amanda Askell" },
                Type = CitationType.InProceedings,
                Year = 2020,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2005.14165",
                Url = "https://arxiv.org/abs/2005.14165",
                Abstract = "We demonstrate that scaling up language models greatly improves task-agnostic, few-shot performance. GPT-3 achieves strong performance on many NLP tasks.",
                Tags = new List<string> { "GPT-3", "few-shot", "language-model", "seminal" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Training language models to follow instructions with human feedback",
                Authors = new List<string> { "Long Ouyang", "Jeff Wu", "Xu Jiang", "Diogo Almeida", "Carroll L. Wainwright", "Pamela Mishkin", "Chong Zhang", "Sandhini Agarwal", "Katarina Slama", "Alex Ray" },
                Type = CitationType.InProceedings,
                Year = 2022,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2203.02155",
                Url = "https://arxiv.org/abs/2203.02155",
                Abstract = "We show an avenue for aligning language models with user intent on a wide range of tasks by fine-tuning with human feedback (InstructGPT).",
                Tags = new List<string> { "RLHF", "instruction-tuning", "alignment", "InstructGPT" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "ReAct: Synergizing Reasoning and Acting in Language Models",
                Authors = new List<string> { "Shunyu Yao", "Jeffrey Zhao", "Dian Yu", "Nan Du", "Izhak Shafran", "Karthik Narasimhan", "Yuan Cao" },
                Type = CitationType.InProceedings,
                Year = 2023,
                JournalOrConference = "International Conference on Learning Representations (ICLR)",
                Doi = "10.48550/arXiv.2210.03629",
                Url = "https://arxiv.org/abs/2210.03629",
                Abstract = "We propose ReAct, a general paradigm to synergize reasoning and acting in language models for solving diverse language reasoning and decision making tasks.",
                Tags = new List<string> { "agents", "reasoning", "tool-use", "ReAct" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Toolformer: Language Models Can Teach Themselves to Use Tools",
                Authors = new List<string> { "Timo Schick", "Jane Dwivedi-Yu", "Roberto Dessì", "Roberta Raileanu", "Maria Lomeli", "Luke Zettlemoyer", "Nicola Cancedda", "Thomas Scialom" },
                Type = CitationType.InProceedings,
                Year = 2023,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2302.04761",
                Url = "https://arxiv.org/abs/2302.04761",
                Abstract = "We introduce Toolformer, a model trained to decide which APIs to call, when to call them, what arguments to pass, and how to best incorporate the results.",
                Tags = new List<string> { "tool-use", "API", "agents", "self-supervised" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "HuggingGPT: Solving AI Tasks with ChatGPT and its Friends in Hugging Face",
                Authors = new List<string> { "Yongliang Shen", "Kaitao Song", "Xu Tan", "Dongsheng Li", "Weiming Lu", "Yueting Zhuang" },
                Type = CitationType.InProceedings,
                Year = 2023,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2303.17580",
                Url = "https://arxiv.org/abs/2303.17580",
                Abstract = "We present HuggingGPT, a system that leverages LLMs to connect various AI models for solving complicated AI tasks.",
                Tags = new List<string> { "agents", "multi-model", "task-planning", "orchestration" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "AutoGPT: An Autonomous GPT-4 Experiment",
                Authors = new List<string> { "Significant Gravitas" },
                Type = CitationType.Website,
                Year = 2023,
                Url = "https://github.com/Significant-Gravitas/AutoGPT",
                Abstract = "AutoGPT is an experimental open-source application showcasing the capabilities of autonomous AI agents powered by GPT-4.",
                Tags = new List<string> { "agents", "autonomous", "GPT-4", "open-source" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Generative Agents: Interactive Simulacra of Human Behavior",
                Authors = new List<string> { "Joon Sung Park", "Joseph C. O'Brien", "Carrie J. Cai", "Meredith Ringel Morris", "Percy Liang", "Michael S. Bernstein" },
                Type = CitationType.InProceedings,
                Year = 2023,
                JournalOrConference = "ACM Symposium on User Interface Software and Technology (UIST)",
                Doi = "10.1145/3586183.3606763",
                Url = "https://arxiv.org/abs/2304.03442",
                Abstract = "We introduce generative agents—computational software agents that simulate believable human behavior using LLMs for memory, planning, and reflection.",
                Tags = new List<string> { "agents", "simulation", "memory", "behavior" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Chain-of-Thought Prompting Elicits Reasoning in Large Language Models",
                Authors = new List<string> { "Jason Wei", "Xuezhi Wang", "Dale Schuurmans", "Maarten Bosma", "Brian Ichter", "Fei Xia", "Ed Chi", "Quoc Le", "Denny Zhou" },
                Type = CitationType.InProceedings,
                Year = 2022,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2201.11903",
                Url = "https://arxiv.org/abs/2201.11903",
                Abstract = "We explore how generating a chain of thought—a series of intermediate reasoning steps—significantly improves the ability of LLMs to perform complex reasoning.",
                Tags = new List<string> { "prompting", "reasoning", "chain-of-thought", "seminal" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Tree of Thoughts: Deliberate Problem Solving with Large Language Models",
                Authors = new List<string> { "Shunyu Yao", "Dian Yu", "Jeffrey Zhao", "Izhak Shafran", "Thomas L. Griffiths", "Yuan Cao", "Karthik Narasimhan" },
                Type = CitationType.InProceedings,
                Year = 2023,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2305.10601",
                Url = "https://arxiv.org/abs/2305.10601",
                Abstract = "We introduce Tree of Thoughts (ToT), which generalizes chain-of-thought prompting and enables exploration over coherent units of text (thoughts).",
                Tags = new List<string> { "prompting", "reasoning", "search", "problem-solving" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "LLaMA: Open and Efficient Foundation Language Models",
                Authors = new List<string> { "Hugo Touvron", "Thibaut Lavril", "Gautier Izacard", "Xavier Martinet", "Marie-Anne Lachaux", "Timothée Lacroix", "Baptiste Rozière", "Naman Goyal", "Eric Hambro", "Faisal Azhar" },
                Type = CitationType.Article,
                Year = 2023,
                JournalOrConference = "arXiv preprint",
                Doi = "10.48550/arXiv.2302.13971",
                Url = "https://arxiv.org/abs/2302.13971",
                Abstract = "We introduce LLaMA, a collection of foundation language models ranging from 7B to 65B parameters trained on publicly available datasets.",
                Tags = new List<string> { "open-source", "foundation-model", "LLaMA", "efficient" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Retrieval-Augmented Generation for Knowledge-Intensive NLP Tasks",
                Authors = new List<string> { "Patrick Lewis", "Ethan Perez", "Aleksandra Piktus", "Fabio Petroni", "Vladimir Karpukhin", "Naman Goyal", "Heinrich Küttler", "Mike Lewis", "Wen-tau Yih", "Tim Rocktäschel" },
                Type = CitationType.InProceedings,
                Year = 2020,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2005.11401",
                Url = "https://arxiv.org/abs/2005.11401",
                Abstract = "We explore a general-purpose fine-tuning recipe for retrieval-augmented generation (RAG) models combining pre-trained parametric and non-parametric memory.",
                Tags = new List<string> { "RAG", "retrieval", "knowledge", "seminal" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Constitutional AI: Harmlessness from AI Feedback",
                Authors = new List<string> { "Yuntao Bai", "Saurav Kadavath", "Sandipan Kundu", "Amanda Askell", "Jackson Kernion", "Andy Jones", "Anna Chen", "Anna Goldie", "Azalia Mirhoseini", "Cameron McKinnon" },
                Type = CitationType.Article,
                Year = 2022,
                JournalOrConference = "arXiv preprint",
                Doi = "10.48550/arXiv.2212.08073",
                Url = "https://arxiv.org/abs/2212.08073",
                Abstract = "We propose Constitutional AI (CAI), a method for training harmless AI assistants without human labels for harmfulness.",
                Tags = new List<string> { "alignment", "safety", "RLAIF", "constitutional" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Evaluating Large Language Models Trained on Code",
                Authors = new List<string> { "Mark Chen", "Jerry Tworek", "Heewoo Jun", "Qiming Yuan", "Henrique Ponde de Oliveira Pinto", "Jared Kaplan", "Harri Edwards", "Yuri Burda", "Nicholas Joseph", "Greg Brockman" },
                Type = CitationType.Article,
                Year = 2021,
                JournalOrConference = "arXiv preprint",
                Doi = "10.48550/arXiv.2107.03374",
                Url = "https://arxiv.org/abs/2107.03374",
                Abstract = "We introduce Codex, a GPT language model fine-tuned on publicly available code from GitHub, and study its Python code-writing capabilities.",
                Tags = new List<string> { "code-generation", "Codex", "GitHub-Copilot", "programming" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "CodeLlama: Open Foundation Models for Code",
                Authors = new List<string> { "Baptiste Rozière", "Jonas Gehring", "Fabian Gloeckle", "Sten Sootla", "Itai Gat", "Xiaoqing Ellen Tan", "Yossi Adi", "Jingyu Liu", "Tal Remez", "Jérémy Rapin" },
                Type = CitationType.Article,
                Year = 2023,
                JournalOrConference = "arXiv preprint",
                Doi = "10.48550/arXiv.2308.12950",
                Url = "https://arxiv.org/abs/2308.12950",
                Abstract = "We release Code Llama, a family of large language models for code based on Llama 2 providing infilling, large context, and instruction-following capabilities.",
                Tags = new List<string> { "code-generation", "open-source", "LLaMA", "programming" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "SWE-bench: Can Language Models Resolve Real-World GitHub Issues?",
                Authors = new List<string> { "Carlos E. Jimenez", "John Yang", "Alexander Wettig", "Shunyu Yao", "Kexin Pei", "Ofir Press", "Karthik Narasimhan" },
                Type = CitationType.InProceedings,
                Year = 2024,
                JournalOrConference = "International Conference on Learning Representations (ICLR)",
                Doi = "10.48550/arXiv.2310.06770",
                Url = "https://arxiv.org/abs/2310.06770",
                Abstract = "We introduce SWE-bench, a benchmark for evaluating LLMs on real-world software engineering problems collected from GitHub issues and pull requests.",
                Tags = new List<string> { "benchmark", "software-engineering", "agents", "evaluation" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "AgentBench: Evaluating LLMs as Agents",
                Authors = new List<string> { "Xiao Liu", "Hao Yu", "Hanchen Zhang", "Yifan Xu", "Xuanyu Lei", "Hanyu Lai", "Yu Gu", "Hangliang Ding", "Kaiwen Men", "Kejuan Yang" },
                Type = CitationType.InProceedings,
                Year = 2023,
                JournalOrConference = "International Conference on Learning Representations (ICLR)",
                Doi = "10.48550/arXiv.2308.03688",
                Url = "https://arxiv.org/abs/2308.03688",
                Abstract = "We present AgentBench, a comprehensive benchmark to evaluate LLMs as agents across diverse environments including web, games, and coding.",
                Tags = new List<string> { "benchmark", "agents", "evaluation", "environments" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Self-Refine: Iterative Refinement with Self-Feedback",
                Authors = new List<string> { "Aman Madaan", "Niket Tandon", "Prakhar Gupta", "Skyler Hallinan", "Luyu Gao", "Sarah Wiegreffe", "Uri Alon", "Nouha Dziri", "Shrimai Prabhumoye", "Yiming Yang" },
                Type = CitationType.InProceedings,
                Year = 2023,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2303.17651",
                Url = "https://arxiv.org/abs/2303.17651",
                Abstract = "We introduce Self-Refine, an approach that allows LLMs to iteratively refine their outputs through self-feedback without additional training.",
                Tags = new List<string> { "self-improvement", "refinement", "feedback", "iterative" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Reflexion: Language Agents with Verbal Reinforcement Learning",
                Authors = new List<string> { "Noah Shinn", "Federico Cassano", "Ashwin Gopinath", "Karthik Narasimhan", "Shunyu Yao" },
                Type = CitationType.InProceedings,
                Year = 2023,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2303.11366",
                Url = "https://arxiv.org/abs/2303.11366",
                Abstract = "We propose Reflexion, a novel framework to reinforce language agents through linguistic feedback rather than updating weights.",
                Tags = new List<string> { "agents", "reinforcement", "reflection", "learning" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "TaskWeaver: A Code-First Agent Framework",
                Authors = new List<string> { "Bo Qiao", "Liqun Li", "Xu Zhang", "Shilin He", "Yu Kang", "Chaoyun Zhang", "Fangkai Yang", "Hang Dong", "Jue Zhang", "Lu Wang" },
                Type = CitationType.Article,
                Year = 2023,
                JournalOrConference = "arXiv preprint",
                Doi = "10.48550/arXiv.2311.17541",
                Url = "https://arxiv.org/abs/2311.17541",
                Abstract = "We introduce TaskWeaver, a code-first agent framework that converts user requests into executable code with plugins as callable functions.",
                Tags = new List<string> { "agents", "code-generation", "framework", "plugins" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Voyager: An Open-Ended Embodied Agent with Large Language Models",
                Authors = new List<string> { "Guanzhi Wang", "Yuqi Xie", "Yunfan Jiang", "Ajay Mandlekar", "Chaowei Xiao", "Yuke Zhu", "Linxi Fan", "Anima Anandkumar" },
                Type = CitationType.Article,
                Year = 2023,
                JournalOrConference = "arXiv preprint",
                Doi = "10.48550/arXiv.2305.16291",
                Url = "https://arxiv.org/abs/2305.16291",
                Abstract = "We introduce Voyager, the first LLM-powered embodied lifelong learning agent in Minecraft that continuously explores, acquires skills, and makes discoveries.",
                Tags = new List<string> { "agents", "embodied", "lifelong-learning", "Minecraft" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "ChatDev: Communicative Agents for Software Development",
                Authors = new List<string> { "Chen Qian", "Xin Cong", "Wei Liu", "Cheng Yang", "Weize Chen", "Yusheng Su", "Yufan Dang", "Jiahao Li", "Juyuan Xu", "Dahai Li" },
                Type = CitationType.InProceedings,
                Year = 2024,
                JournalOrConference = "Association for Computational Linguistics (ACL)",
                Doi = "10.48550/arXiv.2307.07924",
                Url = "https://arxiv.org/abs/2307.07924",
                Abstract = "We present ChatDev, a virtual chat-powered software development company that unifies the software development process through LLM-powered agents.",
                Tags = new List<string> { "agents", "software-development", "multi-agent", "collaboration" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "MetaGPT: Meta Programming for A Multi-Agent Collaborative Framework",
                Authors = new List<string> { "Sirui Hong", "Mingchen Zhuge", "Jonathan Chen", "Xiawu Zheng", "Yuheng Cheng", "Ceyao Zhang", "Jinlin Wang", "Zili Wang", "Steven Ka Shing Yau", "Zijuan Lin" },
                Type = CitationType.InProceedings,
                Year = 2024,
                JournalOrConference = "International Conference on Learning Representations (ICLR)",
                Doi = "10.48550/arXiv.2308.00352",
                Url = "https://arxiv.org/abs/2308.00352",
                Abstract = "We introduce MetaGPT, which encodes human workflows into LLM-based multi-agent collaborations through Standardized Operating Procedures.",
                Tags = new List<string> { "agents", "multi-agent", "meta-programming", "workflows" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "GPT-4 Technical Report",
                Authors = new List<string> { "OpenAI" },
                Type = CitationType.TechReport,
                Year = 2023,
                Publisher = "OpenAI",
                Doi = "10.48550/arXiv.2303.08774",
                Url = "https://arxiv.org/abs/2303.08774",
                Abstract = "We report the development of GPT-4, a large-scale, multimodal model which can accept image and text inputs and produce text outputs.",
                Tags = new List<string> { "GPT-4", "multimodal", "foundation-model", "seminal" },
                DomainId = llmAgents
            },
            new Citation
            {
                Title = "Claude 3 Model Card",
                Authors = new List<string> { "Anthropic" },
                Type = CitationType.TechReport,
                Year = 2024,
                Publisher = "Anthropic",
                Url = "https://www.anthropic.com/news/claude-3-family",
                Abstract = "Claude 3 is Anthropic's family of AI models including Opus, Sonnet, and Haiku, designed to be helpful, harmless, and honest.",
                Tags = new List<string> { "Claude", "foundation-model", "safety", "Anthropic" },
                DomainId = llmAgents
            },

            // ==================== SOFTWARE ENGINEERING ====================
            new Citation
            {
                Title = "Design Patterns: Elements of Reusable Object-Oriented Software",
                Authors = new List<string> { "Erich Gamma", "Richard Helm", "Ralph Johnson", "John Vlissides" },
                Type = CitationType.Book,
                Year = 1994,
                Publisher = "Addison-Wesley",
                Isbn = "978-0201633610",
                Url = "https://www.oreilly.com/library/view/design-patterns-elements/0201633612/",
                Abstract = "The seminal book on design patterns, introducing 23 classic patterns for object-oriented software design.",
                Tags = new List<string> { "design-patterns", "OOP", "seminal", "Gang-of-Four" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "The Mythical Man-Month: Essays on Software Engineering",
                Authors = new List<string> { "Frederick P. Brooks Jr." },
                Type = CitationType.Book,
                Year = 1975,
                Publisher = "Addison-Wesley",
                Isbn = "978-0201835953",
                Url = "https://www.oreilly.com/library/view/mythical-man-month-the/0201835959/",
                Abstract = "Classic essays on software engineering management, including Brooks' Law about adding manpower to late projects.",
                Tags = new List<string> { "management", "classic", "seminal", "Brooks-Law" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                Authors = new List<string> { "Robert C. Martin" },
                Type = CitationType.Book,
                Year = 2008,
                Publisher = "Prentice Hall",
                Isbn = "978-0132350884",
                Url = "https://www.oreilly.com/library/view/clean-code-a/9780136083238/",
                Abstract = "A handbook for writing clean, readable, and maintainable code with practical examples and principles.",
                Tags = new List<string> { "clean-code", "best-practices", "craftsmanship" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Refactoring: Improving the Design of Existing Code",
                Authors = new List<string> { "Martin Fowler" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "Addison-Wesley",
                Isbn = "978-0134757599",
                Url = "https://martinfowler.com/books/refactoring.html",
                Abstract = "The definitive guide to refactoring, explaining how to improve code structure without changing its behavior.",
                Tags = new List<string> { "refactoring", "code-quality", "seminal" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Test-Driven Development: By Example",
                Authors = new List<string> { "Kent Beck" },
                Type = CitationType.Book,
                Year = 2002,
                Publisher = "Addison-Wesley",
                Isbn = "978-0321146533",
                Url = "https://www.oreilly.com/library/view/test-driven-development/0321146530/",
                Abstract = "The seminal book on TDD, demonstrating the red-green-refactor cycle through practical examples.",
                Tags = new List<string> { "TDD", "testing", "agile", "seminal" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                Authors = new List<string> { "Eric Evans" },
                Type = CitationType.Book,
                Year = 2003,
                Publisher = "Addison-Wesley",
                Isbn = "978-0321125217",
                Url = "https://www.domainlanguage.com/ddd/",
                Abstract = "Introduces domain-driven design methodology for managing complexity in software through ubiquitous language and bounded contexts.",
                Tags = new List<string> { "DDD", "architecture", "domain-modeling", "seminal" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Continuous Delivery: Reliable Software Releases through Build, Test, and Deployment Automation",
                Authors = new List<string> { "Jez Humble", "David Farley" },
                Type = CitationType.Book,
                Year = 2010,
                Publisher = "Addison-Wesley",
                Isbn = "978-0321601919",
                Url = "https://continuousdelivery.com/",
                Abstract = "The definitive guide to continuous delivery, covering deployment pipelines, testing strategies, and release automation.",
                Tags = new List<string> { "continuous-delivery", "DevOps", "automation", "seminal" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "The Pragmatic Programmer: Your Journey to Mastery",
                Authors = new List<string> { "David Thomas", "Andrew Hunt" },
                Type = CitationType.Book,
                Year = 2019,
                Publisher = "Addison-Wesley",
                Isbn = "978-0135957059",
                Url = "https://pragprog.com/titles/tpp20/the-pragmatic-programmer-20th-anniversary-edition/",
                Abstract = "Classic advice for software developers on becoming more effective through practical tips and philosophies.",
                Tags = new List<string> { "best-practices", "career", "philosophy" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "A Philosophy of Software Design",
                Authors = new List<string> { "John Ousterhout" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "Yaknyam Press",
                Isbn = "978-1732102200",
                Url = "https://web.stanford.edu/~ouster/cgi-bin/book.php",
                Abstract = "Addresses software complexity and how to minimize it through good design principles and deep modules.",
                Tags = new List<string> { "design", "complexity", "architecture" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Working Effectively with Legacy Code",
                Authors = new List<string> { "Michael Feathers" },
                Type = CitationType.Book,
                Year = 2004,
                Publisher = "Prentice Hall",
                Isbn = "978-0131177055",
                Url = "https://www.oreilly.com/library/view/working-effectively-with/0131177052/",
                Abstract = "Strategies for working with and improving legacy code safely, including techniques for adding tests to untested code.",
                Tags = new List<string> { "legacy-code", "refactoring", "testing" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Extreme Programming Explained: Embrace Change",
                Authors = new List<string> { "Kent Beck", "Cynthia Andres" },
                Type = CitationType.Book,
                Year = 2004,
                Publisher = "Addison-Wesley",
                Isbn = "978-0321278654",
                Url = "https://www.oreilly.com/library/view/extreme-programming-explained/0321278658/",
                Abstract = "The foundational text on Extreme Programming, introducing practices like pair programming, TDD, and continuous integration.",
                Tags = new List<string> { "XP", "agile", "methodology", "seminal" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Software Engineering at Google: Lessons Learned from Programming Over Time",
                Authors = new List<string> { "Titus Winters", "Tom Manshreck", "Hyrum Wright" },
                Type = CitationType.Book,
                Year = 2020,
                Publisher = "O'Reilly Media",
                Isbn = "978-1492082798",
                Url = "https://abseil.io/resources/swe-book",
                Abstract = "How Google builds and maintains software at scale, covering culture, processes, and tools for sustainable engineering.",
                Tags = new List<string> { "Google", "scale", "engineering-practices" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "No Silver Bullet: Essence and Accidents of Software Engineering",
                Authors = new List<string> { "Frederick P. Brooks Jr." },
                Type = CitationType.Article,
                Year = 1987,
                JournalOrConference = "Computer",
                Volume = "20",
                Issue = "4",
                Pages = "10-19",
                Doi = "10.1109/MC.1987.1663532",
                Url = "https://ieeexplore.ieee.org/document/1663532",
                Abstract = "Classic essay arguing there is no single technique that will improve software productivity by an order of magnitude.",
                Tags = new List<string> { "classic", "productivity", "complexity", "seminal" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Out of the Tar Pit",
                Authors = new List<string> { "Ben Moseley", "Peter Marks" },
                Type = CitationType.Article,
                Year = 2006,
                JournalOrConference = "Software Practice Advancement",
                Url = "http://curtclifton.net/papers/MosesleyMarks06a.pdf",
                Abstract = "Analysis of software complexity, distinguishing essential from accidental complexity and proposing functional relational programming.",
                Tags = new List<string> { "complexity", "functional", "state-management" },
                DomainId = softwareEng
            },

            // ==================== ARTIFICIAL INTELLIGENCE ====================
            new Citation
            {
                Title = "Deep Learning",
                Authors = new List<string> { "Ian Goodfellow", "Yoshua Bengio", "Aaron Courville" },
                Type = CitationType.Book,
                Year = 2016,
                Publisher = "MIT Press",
                Isbn = "978-0262035613",
                Url = "https://www.deeplearningbook.org/",
                Abstract = "The comprehensive textbook on deep learning, covering mathematical foundations, techniques, and research perspectives.",
                Tags = new List<string> { "deep-learning", "textbook", "seminal", "neural-networks" },
                DomainId = ai
            },
            new Citation
            {
                Title = "ImageNet Classification with Deep Convolutional Neural Networks",
                Authors = new List<string> { "Alex Krizhevsky", "Ilya Sutskever", "Geoffrey E. Hinton" },
                Type = CitationType.InProceedings,
                Year = 2012,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.1145/3065386",
                Url = "https://papers.nips.cc/paper/2012/hash/c399862d3b9d6b76c8436e924a68c45b-Abstract.html",
                Abstract = "The AlexNet paper that demonstrated deep CNNs could dramatically improve image classification, sparking the deep learning revolution.",
                Tags = new List<string> { "CNN", "ImageNet", "AlexNet", "seminal" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Gradient-Based Learning Applied to Document Recognition",
                Authors = new List<string> { "Yann LeCun", "Léon Bottou", "Yoshua Bengio", "Patrick Haffner" },
                Type = CitationType.Article,
                Year = 1998,
                JournalOrConference = "Proceedings of the IEEE",
                Volume = "86",
                Issue = "11",
                Pages = "2278-2324",
                Doi = "10.1109/5.726791",
                Url = "https://ieeexplore.ieee.org/document/726791",
                Abstract = "Introduces LeNet and demonstrates the effectiveness of convolutional neural networks for handwriting recognition.",
                Tags = new List<string> { "CNN", "LeNet", "seminal", "computer-vision" },
                DomainId = ai
            },
            new Citation
            {
                Title = "BERT: Pre-training of Deep Bidirectional Transformers for Language Understanding",
                Authors = new List<string> { "Jacob Devlin", "Ming-Wei Chang", "Kenton Lee", "Kristina Toutanova" },
                Type = CitationType.InProceedings,
                Year = 2019,
                JournalOrConference = "Conference of the North American Chapter of the Association for Computational Linguistics (NAACL)",
                Doi = "10.48550/arXiv.1810.04805",
                Url = "https://arxiv.org/abs/1810.04805",
                Abstract = "Introduces BERT, which obtains state-of-the-art results on NLP benchmarks through bidirectional pre-training.",
                Tags = new List<string> { "BERT", "NLP", "pre-training", "seminal" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Deep Residual Learning for Image Recognition",
                Authors = new List<string> { "Kaiming He", "Xiangyu Zhang", "Shaoqing Ren", "Jian Sun" },
                Type = CitationType.InProceedings,
                Year = 2016,
                JournalOrConference = "IEEE Conference on Computer Vision and Pattern Recognition (CVPR)",
                Doi = "10.1109/CVPR.2016.90",
                Url = "https://arxiv.org/abs/1512.03385",
                Abstract = "Introduces residual connections enabling training of very deep neural networks, winning ImageNet 2015.",
                Tags = new List<string> { "ResNet", "computer-vision", "seminal", "skip-connections" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Generative Adversarial Nets",
                Authors = new List<string> { "Ian Goodfellow", "Jean Pouget-Abadie", "Mehdi Mirza", "Bing Xu", "David Warde-Farley", "Sherjil Ozair", "Aaron Courville", "Yoshua Bengio" },
                Type = CitationType.InProceedings,
                Year = 2014,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.1406.2661",
                Url = "https://arxiv.org/abs/1406.2661",
                Abstract = "Introduces GANs, a framework for training generative models through adversarial training.",
                Tags = new List<string> { "GAN", "generative", "seminal", "adversarial" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Playing Atari with Deep Reinforcement Learning",
                Authors = new List<string> { "Volodymyr Mnih", "Koray Kavukcuoglu", "David Silver", "Alex Graves", "Ioannis Antonoglou", "Daan Wierstra", "Martin Riedmiller" },
                Type = CitationType.Article,
                Year = 2013,
                JournalOrConference = "arXiv preprint",
                Doi = "10.48550/arXiv.1312.5602",
                Url = "https://arxiv.org/abs/1312.5602",
                Abstract = "Demonstrates deep Q-learning can master Atari games from raw pixels, launching the deep RL revolution.",
                Tags = new List<string> { "DQN", "reinforcement-learning", "games", "seminal" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Mastering the game of Go with deep neural networks and tree search",
                Authors = new List<string> { "David Silver", "Aja Huang", "Chris J. Maddison", "Arthur Guez", "Laurent Sifre", "George van den Driessche", "Julian Schrittwieser", "Ioannis Antonoglou", "Veda Panneershelvam", "Marc Lanctot" },
                Type = CitationType.Article,
                Year = 2016,
                JournalOrConference = "Nature",
                Volume = "529",
                Pages = "484-489",
                Doi = "10.1038/nature16961",
                Url = "https://www.nature.com/articles/nature16961",
                Abstract = "AlphaGo defeats a professional human Go player, combining deep learning with Monte Carlo tree search.",
                Tags = new List<string> { "AlphaGo", "game-playing", "MCTS", "seminal" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Denoising Diffusion Probabilistic Models",
                Authors = new List<string> { "Jonathan Ho", "Ajay Jain", "Pieter Abbeel" },
                Type = CitationType.InProceedings,
                Year = 2020,
                JournalOrConference = "Advances in Neural Information Processing Systems (NeurIPS)",
                Doi = "10.48550/arXiv.2006.11239",
                Url = "https://arxiv.org/abs/2006.11239",
                Abstract = "Presents diffusion models for high-quality image generation, foundational for models like DALL-E and Stable Diffusion.",
                Tags = new List<string> { "diffusion", "generative", "image-synthesis", "seminal" },
                DomainId = ai
            },
            new Citation
            {
                Title = "An Image is Worth 16x16 Words: Transformers for Image Recognition at Scale",
                Authors = new List<string> { "Alexey Dosovitskiy", "Lucas Beyer", "Alexander Kolesnikov", "Dirk Weissenborn", "Xiaohua Zhai", "Thomas Unterthiner", "Mostafa Dehghani", "Matthias Minderer", "Georg Heigold", "Sylvain Gelly" },
                Type = CitationType.InProceedings,
                Year = 2021,
                JournalOrConference = "International Conference on Learning Representations (ICLR)",
                Doi = "10.48550/arXiv.2010.11929",
                Url = "https://arxiv.org/abs/2010.11929",
                Abstract = "Introduces the Vision Transformer (ViT), applying transformers directly to image patches for classification.",
                Tags = new List<string> { "ViT", "transformer", "computer-vision", "seminal" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Learning Transferable Visual Models From Natural Language Supervision",
                Authors = new List<string> { "Alec Radford", "Jong Wook Kim", "Chris Hallacy", "Aditya Ramesh", "Gabriel Goh", "Sandhini Agarwal", "Girish Sastry", "Amanda Askell", "Pamela Mishkin", "Jack Clark" },
                Type = CitationType.InProceedings,
                Year = 2021,
                JournalOrConference = "International Conference on Machine Learning (ICML)",
                Doi = "10.48550/arXiv.2103.00020",
                Url = "https://arxiv.org/abs/2103.00020",
                Abstract = "Introduces CLIP, learning visual concepts from natural language supervision for zero-shot image classification.",
                Tags = new List<string> { "CLIP", "multimodal", "vision-language", "seminal" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Dropout: A Simple Way to Prevent Neural Networks from Overfitting",
                Authors = new List<string> { "Nitish Srivastava", "Geoffrey Hinton", "Alex Krizhevsky", "Ilya Sutskever", "Ruslan Salakhutdinov" },
                Type = CitationType.Article,
                Year = 2014,
                JournalOrConference = "Journal of Machine Learning Research",
                Volume = "15",
                Pages = "1929-1958",
                Url = "https://jmlr.org/papers/v15/srivastava14a.html",
                Abstract = "Introduces dropout regularization, a simple and effective technique to prevent overfitting in neural networks.",
                Tags = new List<string> { "regularization", "dropout", "neural-networks", "seminal" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Adam: A Method for Stochastic Optimization",
                Authors = new List<string> { "Diederik P. Kingma", "Jimmy Ba" },
                Type = CitationType.InProceedings,
                Year = 2015,
                JournalOrConference = "International Conference on Learning Representations (ICLR)",
                Doi = "10.48550/arXiv.1412.6980",
                Url = "https://arxiv.org/abs/1412.6980",
                Abstract = "Introduces the Adam optimizer, combining momentum with adaptive learning rates for efficient training.",
                Tags = new List<string> { "optimization", "Adam", "gradient-descent", "seminal" },
                DomainId = ai
            },
            new Citation
            {
                Title = "Batch Normalization: Accelerating Deep Network Training",
                Authors = new List<string> { "Sergey Ioffe", "Christian Szegedy" },
                Type = CitationType.InProceedings,
                Year = 2015,
                JournalOrConference = "International Conference on Machine Learning (ICML)",
                Doi = "10.48550/arXiv.1502.03167",
                Url = "https://arxiv.org/abs/1502.03167",
                Abstract = "Introduces batch normalization to reduce internal covariate shift and accelerate deep network training.",
                Tags = new List<string> { "normalization", "training", "deep-learning", "seminal" },
                DomainId = ai
            },

            // ==================== SECURITY ====================
            new Citation
            {
                Title = "The Tangled Web: A Guide to Securing Modern Web Applications",
                Authors = new List<string> { "Michal Zalewski" },
                Type = CitationType.Book,
                Year = 2011,
                Publisher = "No Starch Press",
                Isbn = "978-1593273880",
                Url = "https://nostarch.com/tangledweb",
                Abstract = "Comprehensive guide to web security, covering browser security models and common vulnerabilities.",
                Tags = new List<string> { "web-security", "browsers", "vulnerabilities" },
                DomainId = security
            },
            new Citation
            {
                Title = "Smashing The Stack For Fun And Profit",
                Authors = new List<string> { "Aleph One" },
                Type = CitationType.Article,
                Year = 1996,
                JournalOrConference = "Phrack Magazine",
                Volume = "7",
                Issue = "49",
                Url = "http://phrack.org/issues/49/14.html",
                Abstract = "Classic tutorial on stack buffer overflow exploitation, foundational for understanding memory corruption vulnerabilities.",
                Tags = new List<string> { "buffer-overflow", "exploitation", "classic", "seminal" },
                DomainId = security
            },
            new Citation
            {
                Title = "Reflections on Trusting Trust",
                Authors = new List<string> { "Ken Thompson" },
                Type = CitationType.Article,
                Year = 1984,
                JournalOrConference = "Communications of the ACM",
                Volume = "27",
                Issue = "8",
                Pages = "761-763",
                Doi = "10.1145/358198.358210",
                Url = "https://dl.acm.org/doi/10.1145/358198.358210",
                Abstract = "Turing Award lecture demonstrating how compilers can contain hidden backdoors, questioning trust in software.",
                Tags = new List<string> { "trust", "compilers", "backdoor", "classic", "seminal" },
                DomainId = security
            },
            new Citation
            {
                Title = "The Web Application Hacker's Handbook",
                Authors = new List<string> { "Dafydd Stuttard", "Marcus Pinto" },
                Type = CitationType.Book,
                Year = 2011,
                Publisher = "Wiley",
                Isbn = "978-1118026472",
                Url = "https://portswigger.net/web-security/web-application-hackers-handbook",
                Abstract = "Comprehensive guide to finding and exploiting web application security flaws.",
                Tags = new List<string> { "web-security", "penetration-testing", "OWASP" },
                DomainId = security
            },
            new Citation
            {
                Title = "A Method for Obtaining Digital Signatures and Public-Key Cryptosystems",
                Authors = new List<string> { "Ron Rivest", "Adi Shamir", "Leonard Adleman" },
                Type = CitationType.Article,
                Year = 1978,
                JournalOrConference = "Communications of the ACM",
                Volume = "21",
                Issue = "2",
                Pages = "120-126",
                Doi = "10.1145/359340.359342",
                Url = "https://dl.acm.org/doi/10.1145/359340.359342",
                Abstract = "The foundational RSA paper introducing the first practical public-key cryptosystem.",
                Tags = new List<string> { "RSA", "cryptography", "public-key", "seminal" },
                DomainId = security
            },
            new Citation
            {
                Title = "New Directions in Cryptography",
                Authors = new List<string> { "Whitfield Diffie", "Martin Hellman" },
                Type = CitationType.Article,
                Year = 1976,
                JournalOrConference = "IEEE Transactions on Information Theory",
                Volume = "22",
                Issue = "6",
                Pages = "644-654",
                Doi = "10.1109/TIT.1976.1055638",
                Url = "https://ieeexplore.ieee.org/document/1055638",
                Abstract = "Introduces public key cryptography and the Diffie-Hellman key exchange protocol.",
                Tags = new List<string> { "Diffie-Hellman", "cryptography", "key-exchange", "seminal" },
                DomainId = security
            },
            new Citation
            {
                Title = "OWASP Top 10:2025",
                Authors = new List<string> { "OWASP Foundation" },
                Type = CitationType.Standard,
                Year = 2025,
                Publisher = "OWASP",
                Url = "https://owasp.org/Top10/",
                Abstract = "The 2025 edition of OWASP's standard awareness document for developers about the most critical security risks to web applications, including updated categories for AI/ML vulnerabilities and supply chain risks.",
                Tags = new List<string> { "OWASP", "web-security", "vulnerabilities", "standard", "2025" },
                DomainId = security
            },
            new Citation
            {
                Title = "Practical Malware Analysis",
                Authors = new List<string> { "Michael Sikorski", "Andrew Honig" },
                Type = CitationType.Book,
                Year = 2012,
                Publisher = "No Starch Press",
                Isbn = "978-1593272906",
                Url = "https://nostarch.com/malware",
                Abstract = "Hands-on guide to dissecting malicious software, covering static and dynamic analysis techniques.",
                Tags = new List<string> { "malware", "reverse-engineering", "analysis" },
                DomainId = security
            },
            new Citation
            {
                Title = "The Art of Software Security Assessment",
                Authors = new List<string> { "Mark Dowd", "John McDonald", "Justin Schuh" },
                Type = CitationType.Book,
                Year = 2006,
                Publisher = "Addison-Wesley",
                Isbn = "978-0321444424",
                Url = "https://www.oreilly.com/library/view/the-art-of/9780321444424/",
                Abstract = "Comprehensive guide to identifying and understanding software vulnerabilities through code auditing.",
                Tags = new List<string> { "code-audit", "vulnerabilities", "assessment" },
                DomainId = security
            },
            new Citation
            {
                Title = "NIST Cybersecurity Framework",
                Authors = new List<string> { "National Institute of Standards and Technology" },
                Type = CitationType.Standard,
                Year = 2018,
                Publisher = "NIST",
                Doi = "10.6028/NIST.CSWP.04162018",
                Url = "https://www.nist.gov/cyberframework",
                Abstract = "Framework for improving critical infrastructure cybersecurity through standards, guidelines, and practices.",
                Tags = new List<string> { "framework", "compliance", "NIST", "standard" },
                DomainId = security
            },
            new Citation
            {
                Title = "Kerckhoffs's Principle",
                Authors = new List<string> { "Auguste Kerckhoffs" },
                Type = CitationType.Article,
                Year = 1883,
                JournalOrConference = "Journal des sciences militaires",
                Url = "https://en.wikipedia.org/wiki/Kerckhoffs%27s_principle",
                Abstract = "The foundational principle that a cryptosystem should be secure even if everything except the key is public knowledge.",
                Tags = new List<string> { "cryptography", "principles", "classic", "seminal" },
                DomainId = security
            },
            new Citation
            {
                Title = "An Attack on DES",
                Authors = new List<string> { "Eli Biham", "Adi Shamir" },
                Type = CitationType.InProceedings,
                Year = 1991,
                JournalOrConference = "Advances in Cryptology - CRYPTO",
                Doi = "10.1007/3-540-38424-3_1",
                Url = "https://link.springer.com/chapter/10.1007/3-540-38424-3_1",
                Abstract = "Introduces differential cryptanalysis, a powerful technique for attacking block ciphers.",
                Tags = new List<string> { "cryptanalysis", "DES", "differential", "seminal" },
                DomainId = security
            },

            // ==================== DEVOPS & LIFECYCLE ====================
            new Citation
            {
                Title = "The Phoenix Project: A Novel about IT, DevOps, and Helping Your Business Win",
                Authors = new List<string> { "Gene Kim", "Kevin Behr", "George Spafford" },
                Type = CitationType.Book,
                Year = 2013,
                Publisher = "IT Revolution Press",
                Isbn = "978-0988262591",
                Url = "https://itrevolution.com/product/the-phoenix-project/",
                Abstract = "A novel that illustrates DevOps principles through the story of an IT manager's journey to transform his organization.",
                Tags = new List<string> { "DevOps", "novel", "transformation", "seminal" },
                DomainId = devops
            },
            new Citation
            {
                Title = "The DevOps Handbook",
                Authors = new List<string> { "Gene Kim", "Patrick Debois", "John Willis", "Jez Humble" },
                Type = CitationType.Book,
                Year = 2016,
                Publisher = "IT Revolution Press",
                Isbn = "978-1942788003",
                Url = "https://itrevolution.com/product/the-devops-handbook-second-edition/",
                Abstract = "Practical guide to implementing DevOps principles with case studies from high-performing organizations.",
                Tags = new List<string> { "DevOps", "practices", "handbook", "seminal" },
                DomainId = devops
            },
            new Citation
            {
                Title = "Site Reliability Engineering: How Google Runs Production Systems",
                Authors = new List<string> { "Betsy Beyer", "Chris Jones", "Jennifer Petoff", "Niall Richard Murphy" },
                Type = CitationType.Book,
                Year = 2016,
                Publisher = "O'Reilly Media",
                Isbn = "978-1491929124",
                Url = "https://sre.google/sre-book/table-of-contents/",
                Abstract = "How Google applies software engineering practices to operations, defining the SRE discipline.",
                Tags = new List<string> { "SRE", "Google", "operations", "seminal" },
                DomainId = devops
            },
            new Citation
            {
                Title = "Accelerate: Building and Scaling High Performing Technology Organizations",
                Authors = new List<string> { "Nicole Forsgren", "Jez Humble", "Gene Kim" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "IT Revolution Press",
                Isbn = "978-1942788331",
                Url = "https://itrevolution.com/product/accelerate/",
                Abstract = "Research-backed book identifying capabilities that drive software delivery performance and organizational success.",
                Tags = new List<string> { "DORA", "metrics", "performance", "research" },
                DomainId = devops
            },
            new Citation
            {
                Title = "Infrastructure as Code: Managing Servers in the Cloud",
                Authors = new List<string> { "Kief Morris" },
                Type = CitationType.Book,
                Year = 2020,
                Publisher = "O'Reilly Media",
                Isbn = "978-1492057666",
                Url = "https://www.oreilly.com/library/view/infrastructure-as-code/9781098114664/",
                Abstract = "Principles and practices for managing infrastructure using code and automation tools.",
                Tags = new List<string> { "IaC", "automation", "cloud", "Terraform" },
                DomainId = devops
            },
            new Citation
            {
                Title = "Kubernetes: Up and Running",
                Authors = new List<string> { "Brendan Burns", "Joe Beda", "Kelsey Hightower" },
                Type = CitationType.Book,
                Year = 2022,
                Publisher = "O'Reilly Media",
                Isbn = "978-1098110208",
                Url = "https://www.oreilly.com/library/view/kubernetes-up-and/9781098110192/",
                Abstract = "Comprehensive guide to deploying and managing containerized applications with Kubernetes.",
                Tags = new List<string> { "Kubernetes", "containers", "orchestration" },
                DomainId = devops
            },
            new Citation
            {
                Title = "The Twelve-Factor App",
                Authors = new List<string> { "Adam Wiggins" },
                Type = CitationType.Website,
                Year = 2012,
                Url = "https://12factor.net/",
                Abstract = "Methodology for building software-as-a-service apps that are portable, scalable, and suitable for modern cloud platforms.",
                Tags = new List<string> { "cloud-native", "methodology", "SaaS", "seminal" },
                DomainId = devops
            },
            new Citation
            {
                Title = "Release It! Design and Deploy Production-Ready Software",
                Authors = new List<string> { "Michael T. Nygard" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "Pragmatic Bookshelf",
                Isbn = "978-1680502398",
                Url = "https://pragprog.com/titles/mnee2/release-it-second-edition/",
                Abstract = "Patterns for designing and deploying production-ready systems that survive real-world failures.",
                Tags = new List<string> { "reliability", "patterns", "production" },
                DomainId = devops
            },
            new Citation
            {
                Title = "Team Topologies: Organizing Business and Technology Teams for Fast Flow",
                Authors = new List<string> { "Matthew Skelton", "Manuel Pais" },
                Type = CitationType.Book,
                Year = 2019,
                Publisher = "IT Revolution Press",
                Isbn = "978-1942788812",
                Url = "https://teamtopologies.com/book",
                Abstract = "Framework for organizing teams to optimize for fast flow of change while minimizing cognitive load.",
                Tags = new List<string> { "team-design", "organization", "flow" },
                DomainId = devops
            },
            new Citation
            {
                Title = "DORA State of DevOps Report",
                Authors = new List<string> { "DORA Team", "Google Cloud" },
                Type = CitationType.TechReport,
                Year = 2023,
                Publisher = "Google Cloud",
                Url = "https://dora.dev/research/",
                Abstract = "Annual research report on DevOps practices and their impact on software delivery and organizational performance.",
                Tags = new List<string> { "DORA", "research", "metrics", "annual" },
                DomainId = devops
            },

            // ==================== DISTRIBUTED SYSTEMS ====================
            new Citation
            {
                Title = "Designing Data-Intensive Applications",
                Authors = new List<string> { "Martin Kleppmann" },
                Type = CitationType.Book,
                Year = 2017,
                Publisher = "O'Reilly Media",
                Isbn = "978-1449373320",
                Url = "https://dataintensive.net/",
                Abstract = "Comprehensive guide to the principles and practices of building reliable, scalable, and maintainable data systems.",
                Tags = new List<string> { "distributed-systems", "databases", "seminal", "architecture" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "The Google File System",
                Authors = new List<string> { "Sanjay Ghemawat", "Howard Gobioff", "Shun-Tak Leung" },
                Type = CitationType.InProceedings,
                Year = 2003,
                JournalOrConference = "ACM Symposium on Operating Systems Principles (SOSP)",
                Doi = "10.1145/945445.945450",
                Url = "https://static.googleusercontent.com/media/research.google.com/en//archive/gfs-sosp2003.pdf",
                Abstract = "Describes GFS, Google's scalable distributed file system designed for large distributed data-intensive applications.",
                Tags = new List<string> { "GFS", "Google", "storage", "seminal" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "MapReduce: Simplified Data Processing on Large Clusters",
                Authors = new List<string> { "Jeffrey Dean", "Sanjay Ghemawat" },
                Type = CitationType.InProceedings,
                Year = 2004,
                JournalOrConference = "OSDI",
                Doi = "10.1145/1327452.1327492",
                Url = "https://static.googleusercontent.com/media/research.google.com/en//archive/mapreduce-osdi04.pdf",
                Abstract = "Programming model for processing and generating large data sets with a parallel, distributed algorithm.",
                Tags = new List<string> { "MapReduce", "Google", "big-data", "seminal" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "Bigtable: A Distributed Storage System for Structured Data",
                Authors = new List<string> { "Fay Chang", "Jeffrey Dean", "Sanjay Ghemawat", "Wilson C. Hsieh", "Deborah A. Wallach", "Mike Burrows", "Tushar Chandra", "Andrew Fikes", "Robert E. Gruber" },
                Type = CitationType.InProceedings,
                Year = 2006,
                JournalOrConference = "OSDI",
                Doi = "10.1145/1365815.1365816",
                Url = "https://static.googleusercontent.com/media/research.google.com/en//archive/bigtable-osdi06.pdf",
                Abstract = "Describes Bigtable, a distributed storage system for managing structured data at massive scale.",
                Tags = new List<string> { "Bigtable", "Google", "NoSQL", "seminal" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "Dynamo: Amazon's Highly Available Key-value Store",
                Authors = new List<string> { "Giuseppe DeCandia", "Deniz Hastorun", "Madan Jampani", "Gunavardhan Kakulapati", "Avinash Lakshman", "Alex Pilchin", "Swaminathan Sivasubramanian", "Peter Vosshall", "Werner Vogels" },
                Type = CitationType.InProceedings,
                Year = 2007,
                JournalOrConference = "ACM Symposium on Operating Systems Principles (SOSP)",
                Doi = "10.1145/1294261.1294281",
                Url = "https://www.allthingsdistributed.com/files/amazon-dynamo-sosp2007.pdf",
                Abstract = "Describes Amazon's Dynamo, a highly available key-value store that sacrifices consistency for availability.",
                Tags = new List<string> { "Dynamo", "Amazon", "eventual-consistency", "seminal" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "CAP Twelve Years Later: How the Rules Have Changed",
                Authors = new List<string> { "Eric Brewer" },
                Type = CitationType.Article,
                Year = 2012,
                JournalOrConference = "Computer",
                Volume = "45",
                Issue = "2",
                Pages = "23-29",
                Doi = "10.1109/MC.2012.37",
                Url = "https://ieeexplore.ieee.org/document/6133253",
                Abstract = "Revisits the CAP theorem, clarifying misconceptions and discussing practical implications for distributed systems.",
                Tags = new List<string> { "CAP", "theorem", "consistency", "availability" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "The Part-Time Parliament",
                Authors = new List<string> { "Leslie Lamport" },
                Type = CitationType.Article,
                Year = 1998,
                JournalOrConference = "ACM Transactions on Computer Systems",
                Volume = "16",
                Issue = "2",
                Pages = "133-169",
                Doi = "10.1145/279227.279229",
                Url = "https://lamport.azurewebsites.net/pubs/lamport-paxos.pdf",
                Abstract = "Describes the Paxos protocol for achieving consensus in a network of unreliable processors.",
                Tags = new List<string> { "Paxos", "consensus", "distributed", "seminal" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "In Search of an Understandable Consensus Algorithm",
                Authors = new List<string> { "Diego Ongaro", "John Ousterhout" },
                Type = CitationType.InProceedings,
                Year = 2014,
                JournalOrConference = "USENIX Annual Technical Conference",
                Url = "https://raft.github.io/raft.pdf",
                Abstract = "Introduces Raft, a consensus algorithm designed to be easier to understand than Paxos.",
                Tags = new List<string> { "Raft", "consensus", "distributed", "seminal" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "Spanner: Google's Globally-Distributed Database",
                Authors = new List<string> { "James C. Corbett", "Jeffrey Dean", "Michael Epstein", "Andrew Fikes", "Christopher Frost", "J. J. Furman", "Sanjay Ghemawat", "Andrey Gubarev", "Christopher Heiser", "Peter Hochschild" },
                Type = CitationType.InProceedings,
                Year = 2012,
                JournalOrConference = "OSDI",
                Doi = "10.1145/2491245",
                Url = "https://static.googleusercontent.com/media/research.google.com/en//archive/spanner-osdi2012.pdf",
                Abstract = "Describes Spanner, Google's scalable, globally distributed database with external consistency.",
                Tags = new List<string> { "Spanner", "Google", "NewSQL", "seminal" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "Kafka: a Distributed Messaging System for Log Processing",
                Authors = new List<string> { "Jay Kreps", "Neha Narkhede", "Jun Rao" },
                Type = CitationType.InProceedings,
                Year = 2011,
                JournalOrConference = "NetDB Workshop",
                Url = "https://www.microsoft.com/en-us/research/wp-content/uploads/2017/09/Kafka.pdf",
                Abstract = "Introduces Apache Kafka, a distributed messaging system designed for high-throughput log processing.",
                Tags = new List<string> { "Kafka", "messaging", "streaming", "seminal" },
                DomainId = distributed
            },
            new Citation
            {
                Title = "Time, Clocks, and the Ordering of Events in a Distributed System",
                Authors = new List<string> { "Leslie Lamport" },
                Type = CitationType.Article,
                Year = 1978,
                JournalOrConference = "Communications of the ACM",
                Volume = "21",
                Issue = "7",
                Pages = "558-565",
                Doi = "10.1145/359545.359563",
                Url = "https://lamport.azurewebsites.net/pubs/time-clocks.pdf",
                Abstract = "Foundational paper on logical clocks and the happens-before relation in distributed systems.",
                Tags = new List<string> { "logical-clocks", "ordering", "causality", "seminal" },
                DomainId = distributed
            },

            // ==================== DATA ENGINEERING ====================
            new Citation
            {
                Title = "The Data Warehouse Toolkit",
                Authors = new List<string> { "Ralph Kimball", "Margy Ross" },
                Type = CitationType.Book,
                Year = 2013,
                Publisher = "Wiley",
                Isbn = "978-1118530801",
                Url = "https://www.kimballgroup.com/data-warehouse-business-intelligence-resources/books/data-warehouse-dw-toolkit/",
                Abstract = "The definitive guide to dimensional modeling for data warehouses, covering star schemas and ETL best practices.",
                Tags = new List<string> { "data-warehouse", "dimensional-modeling", "ETL", "seminal" },
                DomainId = dataEng
            },
            new Citation
            {
                Title = "Spark: The Definitive Guide",
                Authors = new List<string> { "Bill Chambers", "Matei Zaharia" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "O'Reilly Media",
                Isbn = "978-1491912218",
                Url = "https://www.oreilly.com/library/view/spark-the-definitive/9781491912201/",
                Abstract = "Comprehensive guide to Apache Spark for big data processing and analytics.",
                Tags = new List<string> { "Spark", "big-data", "processing" },
                DomainId = dataEng
            },
            new Citation
            {
                Title = "Streaming Systems",
                Authors = new List<string> { "Tyler Akidau", "Slava Chernyak", "Reuven Lax" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "O'Reilly Media",
                Isbn = "978-1491983874",
                Url = "https://www.oreilly.com/library/view/streaming-systems/9781491983867/",
                Abstract = "The what, where, when, and how of large-scale data processing with streaming systems.",
                Tags = new List<string> { "streaming", "real-time", "data-processing" },
                DomainId = dataEng
            },
            new Citation
            {
                Title = "Fundamentals of Data Engineering",
                Authors = new List<string> { "Joe Reis", "Matt Housley" },
                Type = CitationType.Book,
                Year = 2022,
                Publisher = "O'Reilly Media",
                Isbn = "978-1098108304",
                Url = "https://www.oreilly.com/library/view/fundamentals-of-data/9781098108298/",
                Abstract = "Planning and building data systems that are fit for purpose, covering the data engineering lifecycle.",
                Tags = new List<string> { "data-engineering", "lifecycle", "architecture" },
                DomainId = dataEng
            },
            new Citation
            {
                Title = "The Data Lakehouse: A New Paradigm",
                Authors = new List<string> { "Michael Armbrust", "Ali Ghodsi", "Reynold Xin", "Matei Zaharia" },
                Type = CitationType.Article,
                Year = 2021,
                JournalOrConference = "CIDR",
                Url = "https://www.cidrdb.org/cidr2021/papers/cidr2021_paper17.pdf",
                Abstract = "Introduces the lakehouse architecture combining data lake and data warehouse capabilities.",
                Tags = new List<string> { "lakehouse", "Delta-Lake", "architecture" },
                DomainId = dataEng
            },
            new Citation
            {
                Title = "The Semantic Lakehouse: A New Paradigm for Data Management",
                Authors = new List<string> { "Databricks" },
                Type = CitationType.Website,
                Year = 2023,
                Url = "https://www.databricks.com/glossary/data-lakehouse",
                Abstract = "Overview of the data lakehouse pattern combining the best of data warehouses and data lakes.",
                Tags = new List<string> { "lakehouse", "Databricks", "architecture" },
                DomainId = dataEng
            },
            new Citation
            {
                Title = "Building Evolutionary Architectures",
                Authors = new List<string> { "Neal Ford", "Rebecca Parsons", "Patrick Kua" },
                Type = CitationType.Book,
                Year = 2017,
                Publisher = "O'Reilly Media",
                Isbn = "978-1491986363",
                Url = "https://www.oreilly.com/library/view/building-evolutionary-architectures/9781491986356/",
                Abstract = "Support constant change in software systems through evolutionary architecture and fitness functions.",
                Tags = new List<string> { "architecture", "evolution", "fitness-functions" },
                DomainId = dataEng
            },

            // ==================== HCI ====================
            new Citation
            {
                Title = "The Design of Everyday Things",
                Authors = new List<string> { "Don Norman" },
                Type = CitationType.Book,
                Year = 2013,
                Publisher = "Basic Books",
                Isbn = "978-0465050659",
                Url = "https://www.nngroup.com/books/design-everyday-things-revised/",
                Abstract = "Classic text on human-centered design, introducing concepts like affordances and signifiers.",
                Tags = new List<string> { "design", "usability", "seminal", "HCI" },
                DomainId = hci
            },
            new Citation
            {
                Title = "Don't Make Me Think",
                Authors = new List<string> { "Steve Krug" },
                Type = CitationType.Book,
                Year = 2014,
                Publisher = "New Riders",
                Isbn = "978-0321965516",
                Url = "https://sensible.com/dont-make-me-think/",
                Abstract = "Common sense approach to web usability, emphasizing intuitive navigation and clear communication.",
                Tags = new List<string> { "usability", "web-design", "UX" },
                DomainId = hci
            },
            new Citation
            {
                Title = "About Face: The Essentials of Interaction Design",
                Authors = new List<string> { "Alan Cooper", "Robert Reimann", "David Cronin", "Christopher Noessel" },
                Type = CitationType.Book,
                Year = 2014,
                Publisher = "Wiley",
                Isbn = "978-1118766576",
                Url = "https://www.wiley.com/en-us/About+Face:+The+Essentials+of+Interaction+Design,+4th+Edition-p-9781118766576",
                Abstract = "Comprehensive guide to interaction design principles and goal-directed design methodology.",
                Tags = new List<string> { "interaction-design", "personas", "methodology" },
                DomainId = hci
            },
            new Citation
            {
                Title = "Web Content Accessibility Guidelines (WCAG) 2.1",
                Authors = new List<string> { "World Wide Web Consortium (W3C)" },
                Type = CitationType.Standard,
                Year = 2018,
                Publisher = "W3C",
                Url = "https://www.w3.org/TR/WCAG21/",
                Abstract = "International standard for making web content accessible to people with disabilities.",
                Tags = new List<string> { "accessibility", "WCAG", "standard", "W3C" },
                DomainId = hci
            },
            new Citation
            {
                Title = "Inclusive Design Patterns",
                Authors = new List<string> { "Heydon Pickering" },
                Type = CitationType.Book,
                Year = 2016,
                Publisher = "Smashing Magazine",
                Isbn = "978-3945749432",
                Url = "https://www.smashingmagazine.com/inclusive-design-patterns/",
                Abstract = "Practical patterns for designing accessible interfaces for diverse users.",
                Tags = new List<string> { "accessibility", "inclusive-design", "patterns" },
                DomainId = hci
            },
            new Citation
            {
                Title = "Laws of UX",
                Authors = new List<string> { "Jon Yablonski" },
                Type = CitationType.Book,
                Year = 2020,
                Publisher = "O'Reilly Media",
                Isbn = "978-1492055310",
                Url = "https://lawsofux.com/",
                Abstract = "Collection of design principles that help understand how users perceive and interact with interfaces.",
                Tags = new List<string> { "UX", "psychology", "principles" },
                DomainId = hci
            },
            new Citation
            {
                Title = "Hooked: How to Build Habit-Forming Products",
                Authors = new List<string> { "Nir Eyal" },
                Type = CitationType.Book,
                Year = 2014,
                Publisher = "Portfolio",
                Isbn = "978-1591847786",
                Url = "https://www.nirandfar.com/hooked/",
                Abstract = "Framework for building products that create user habits through the Hook Model.",
                Tags = new List<string> { "product-design", "habits", "engagement" },
                DomainId = hci
            },

            // ==================== CLOUD COMPUTING ====================
            new Citation
            {
                Title = "Cloud Native Patterns",
                Authors = new List<string> { "Cornelia Davis" },
                Type = CitationType.Book,
                Year = 2019,
                Publisher = "Manning",
                Isbn = "978-1617294297",
                Url = "https://www.manning.com/books/cloud-native-patterns",
                Abstract = "Designing change-tolerant software for cloud platforms using patterns for redundancy, scalability, and resilience.",
                Tags = new List<string> { "cloud-native", "patterns", "microservices" },
                DomainId = cloud
            },
            new Citation
            {
                Title = "Building Microservices",
                Authors = new List<string> { "Sam Newman" },
                Type = CitationType.Book,
                Year = 2021,
                Publisher = "O'Reilly Media",
                Isbn = "978-1492034025",
                Url = "https://www.oreilly.com/library/view/building-microservices-2nd/9781492034018/",
                Abstract = "Designing fine-grained systems with practical advice on decomposition, integration, and deployment.",
                Tags = new List<string> { "microservices", "architecture", "distributed" },
                DomainId = cloud
            },
            new Citation
            {
                Title = "AWS Well-Architected Framework",
                Authors = new List<string> { "Amazon Web Services" },
                Type = CitationType.Standard,
                Year = 2024,
                Publisher = "AWS",
                Url = "https://docs.aws.amazon.com/wellarchitected/latest/framework/welcome.html",
                Abstract = "Best practices for designing and operating reliable, secure, efficient, cost-effective, and sustainable systems in AWS.",
                Tags = new List<string> { "AWS", "cloud", "architecture", "best-practices" },
                DomainId = cloud
            },
            new Citation
            {
                Title = "Cloud Architecture Patterns",
                Authors = new List<string> { "Bill Wilder" },
                Type = CitationType.Book,
                Year = 2012,
                Publisher = "O'Reilly Media",
                Isbn = "978-1449319779",
                Url = "https://www.oreilly.com/library/view/cloud-architecture-patterns/9781449357979/",
                Abstract = "Using Microsoft Azure to build scalable and resilient cloud applications.",
                Tags = new List<string> { "Azure", "cloud", "patterns", "scalability" },
                DomainId = cloud
            },
            new Citation
            {
                Title = "Serverless Architectures on AWS",
                Authors = new List<string> { "Peter Sbarski", "Yan Cui", "Ajay Nair" },
                Type = CitationType.Book,
                Year = 2022,
                Publisher = "Manning",
                Isbn = "978-1617295423",
                Url = "https://www.manning.com/books/serverless-architectures-on-aws-second-edition",
                Abstract = "Building event-driven applications using Lambda, API Gateway, and other AWS serverless services.",
                Tags = new List<string> { "serverless", "AWS", "Lambda", "architecture" },
                DomainId = cloud
            },
            new Citation
            {
                Title = "Google Cloud Platform in Action",
                Authors = new List<string> { "JJ Geewax" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "Manning",
                Isbn = "978-1617293528",
                Url = "https://www.manning.com/books/google-cloud-platform-in-action",
                Abstract = "Guide to building applications on Google Cloud Platform covering compute, storage, and machine learning services.",
                Tags = new List<string> { "GCP", "cloud", "Google", "platform" },
                DomainId = cloud
            },

            // ==================== NETWORKING ====================
            new Citation
            {
                Title = "Computer Networking: A Top-Down Approach",
                Authors = new List<string> { "James Kurose", "Keith Ross" },
                Type = CitationType.Book,
                Year = 2021,
                Publisher = "Pearson",
                Isbn = "978-0136681557",
                Url = "https://gaia.cs.umass.edu/kurose_ross/index.php",
                Abstract = "Comprehensive networking textbook covering application, transport, network, and link layers.",
                Tags = new List<string> { "networking", "textbook", "TCP/IP", "protocols" },
                DomainId = networking
            },
            new Citation
            {
                Title = "TCP/IP Illustrated, Volume 1: The Protocols",
                Authors = new List<string> { "W. Richard Stevens", "Kevin R. Fall" },
                Type = CitationType.Book,
                Year = 2011,
                Publisher = "Addison-Wesley",
                Isbn = "978-0321336316",
                Url = "https://www.oreilly.com/library/view/tcpip-illustrated-volume/9780132808200/",
                Abstract = "Detailed examination of TCP/IP protocol suite with packet traces and implementation details.",
                Tags = new List<string> { "TCP/IP", "protocols", "seminal", "networking" },
                DomainId = networking
            },
            new Citation
            {
                Title = "HTTP/3 Explained",
                Authors = new List<string> { "Daniel Stenberg" },
                Type = CitationType.Website,
                Year = 2023,
                Url = "https://http3-explained.haxx.se/",
                Abstract = "Comprehensive guide to HTTP/3 and QUIC protocol explaining the next generation of web transport.",
                Tags = new List<string> { "HTTP/3", "QUIC", "protocols", "web" },
                DomainId = networking
            },
            new Citation
            {
                Title = "High Performance Browser Networking",
                Authors = new List<string> { "Ilya Grigorik" },
                Type = CitationType.Book,
                Year = 2013,
                Publisher = "O'Reilly Media",
                Isbn = "978-1449344764",
                Url = "https://hpbn.co/",
                Abstract = "Guide to networking and performance optimization for web applications and mobile apps.",
                Tags = new List<string> { "performance", "networking", "browser", "optimization" },
                DomainId = networking
            },
            new Citation
            {
                Title = "The End-to-End Arguments in System Design",
                Authors = new List<string> { "Jerome H. Saltzer", "David P. Reed", "David D. Clark" },
                Type = CitationType.Article,
                Year = 1984,
                JournalOrConference = "ACM Transactions on Computer Systems",
                Volume = "2",
                Issue = "4",
                Pages = "277-288",
                Doi = "10.1145/357401.357402",
                Url = "https://dl.acm.org/doi/10.1145/357401.357402",
                Abstract = "Foundational paper on the end-to-end principle in network design, influencing internet architecture.",
                Tags = new List<string> { "end-to-end", "architecture", "seminal", "internet" },
                DomainId = networking
            },
            new Citation
            {
                Title = "A Protocol for Packet Network Intercommunication",
                Authors = new List<string> { "Vinton Cerf", "Robert Kahn" },
                Type = CitationType.Article,
                Year = 1974,
                JournalOrConference = "IEEE Transactions on Communications",
                Volume = "22",
                Issue = "5",
                Pages = "637-648",
                Doi = "10.1109/TCOM.1974.1092259",
                Url = "https://ieeexplore.ieee.org/document/1092259",
                Abstract = "The foundational paper describing TCP, the basis for internet communication.",
                Tags = new List<string> { "TCP", "internet", "seminal", "protocol" },
                DomainId = networking
            },

            // ==================== DATABASE SYSTEMS ====================
            new Citation
            {
                Title = "Database Internals: A Deep Dive into How Distributed Data Systems Work",
                Authors = new List<string> { "Alex Petrov" },
                Type = CitationType.Book,
                Year = 2019,
                Publisher = "O'Reilly Media",
                Isbn = "978-1492040347",
                Url = "https://www.databass.dev/",
                Abstract = "Deep exploration of database storage engines, distributed systems concepts, and consistency models.",
                Tags = new List<string> { "databases", "internals", "distributed", "storage" },
                DomainId = databases
            },
            new Citation
            {
                Title = "A Relational Model of Data for Large Shared Data Banks",
                Authors = new List<string> { "Edgar F. Codd" },
                Type = CitationType.Article,
                Year = 1970,
                JournalOrConference = "Communications of the ACM",
                Volume = "13",
                Issue = "6",
                Pages = "377-387",
                Doi = "10.1145/362384.362685",
                Url = "https://dl.acm.org/doi/10.1145/362384.362685",
                Abstract = "The foundational paper introducing the relational model, transforming database theory and practice.",
                Tags = new List<string> { "relational", "SQL", "seminal", "theory" },
                DomainId = databases
            },
            new Citation
            {
                Title = "SQL Performance Explained",
                Authors = new List<string> { "Markus Winand" },
                Type = CitationType.Book,
                Year = 2012,
                Publisher = "Self-published",
                Url = "https://use-the-index-luke.com/",
                Abstract = "Everything developers need to know about SQL performance, focusing on index usage and query optimization.",
                Tags = new List<string> { "SQL", "performance", "indexing", "optimization" },
                DomainId = databases
            },
            new Citation
            {
                Title = "Seven Databases in Seven Weeks",
                Authors = new List<string> { "Luc Perkins", "Eric Redmond", "Jim R. Wilson" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "Pragmatic Bookshelf",
                Isbn = "978-1680502534",
                Url = "https://pragprog.com/titles/pwrdata/seven-databases-in-seven-weeks-second-edition/",
                Abstract = "Practical introduction to Redis, Neo4j, CouchDB, MongoDB, HBase, Postgres, and DynamoDB.",
                Tags = new List<string> { "NoSQL", "databases", "polyglot", "comparison" },
                DomainId = databases
            },
            new Citation
            {
                Title = "Architecture of a Database System",
                Authors = new List<string> { "Joseph M. Hellerstein", "Michael Stonebraker", "James Hamilton" },
                Type = CitationType.Article,
                Year = 2007,
                JournalOrConference = "Foundations and Trends in Databases",
                Volume = "1",
                Issue = "2",
                Pages = "141-259",
                Doi = "10.1561/1900000002",
                Url = "https://dsf.berkeley.edu/papers/fntdb07-architecture.pdf",
                Abstract = "Comprehensive overview of relational DBMS architecture covering query processing, storage, and transactions.",
                Tags = new List<string> { "architecture", "RDBMS", "internals", "survey" },
                DomainId = databases
            },
            new Citation
            {
                Title = "CockroachDB: The Resilient Geo-Distributed SQL Database",
                Authors = new List<string> { "Rebecca Taft", "Irfan Sharif", "Andrei Matei", "Nathan VanBenschoten", "Jordan Lewis", "Tobias Grieger", "Kai Niemi", "Andy Woods", "Anne Biber", "Raphael Poss" },
                Type = CitationType.InProceedings,
                Year = 2020,
                JournalOrConference = "SIGMOD",
                Doi = "10.1145/3318464.3386134",
                Url = "https://dl.acm.org/doi/10.1145/3318464.3386134",
                Abstract = "Design and implementation of CockroachDB, a geo-distributed SQL database with serializable isolation.",
                Tags = new List<string> { "CockroachDB", "distributed", "SQL", "NewSQL" },
                DomainId = databases
            },

            // ==================== PROGRAMMING LANGUAGES ====================
            new Citation
            {
                Title = "Structure and Interpretation of Computer Programs",
                Authors = new List<string> { "Harold Abelson", "Gerald Jay Sussman" },
                Type = CitationType.Book,
                Year = 1996,
                Publisher = "MIT Press",
                Isbn = "978-0262510875",
                Url = "https://mitp-content-server.mit.edu/books/content/sectbyfn/books_pres_0/6515/sicp.zip/index.html",
                Abstract = "Classic computer science textbook teaching programming through Scheme and fundamental concepts.",
                Tags = new List<string> { "SICP", "Scheme", "seminal", "fundamentals" },
                DomainId = progLang
            },
            new Citation
            {
                Title = "Types and Programming Languages",
                Authors = new List<string> { "Benjamin C. Pierce" },
                Type = CitationType.Book,
                Year = 2002,
                Publisher = "MIT Press",
                Isbn = "978-0262162098",
                Url = "https://www.cis.upenn.edu/~bcpierce/tapl/",
                Abstract = "Comprehensive introduction to type systems covering lambda calculus, polymorphism, and subtyping.",
                Tags = new List<string> { "type-theory", "lambda-calculus", "seminal", "theory" },
                DomainId = progLang
            },
            new Citation
            {
                Title = "Compilers: Principles, Techniques, and Tools",
                Authors = new List<string> { "Alfred Aho", "Monica Lam", "Ravi Sethi", "Jeffrey Ullman" },
                Type = CitationType.Book,
                Year = 2006,
                Publisher = "Pearson",
                Isbn = "978-0321486813",
                Url = "https://www.pearson.com/en-us/subject-catalog/p/compilers-principles-techniques-and-tools/P200000003513",
                Abstract = "The definitive textbook on compiler construction, known as the Dragon Book.",
                Tags = new List<string> { "compilers", "parsing", "seminal", "Dragon-Book" },
                DomainId = progLang
            },
            new Citation
            {
                Title = "Programming Language Pragmatics",
                Authors = new List<string> { "Michael L. Scott" },
                Type = CitationType.Book,
                Year = 2015,
                Publisher = "Morgan Kaufmann",
                Isbn = "978-0124104099",
                Url = "https://www.cs.rochester.edu/~scott/pragmatics/",
                Abstract = "Comprehensive treatment of programming language design and implementation covering syntax, semantics, and pragmatics.",
                Tags = new List<string> { "languages", "design", "textbook", "semantics" },
                DomainId = progLang
            },
            new Citation
            {
                Title = "The Rust Programming Language",
                Authors = new List<string> { "Steve Klabnik", "Carol Nichols" },
                Type = CitationType.Book,
                Year = 2023,
                Publisher = "No Starch Press",
                Isbn = "978-1718503106",
                Url = "https://doc.rust-lang.org/book/",
                Abstract = "The official book for learning Rust, covering ownership, borrowing, and systems programming.",
                Tags = new List<string> { "Rust", "systems", "memory-safety", "official" },
                DomainId = progLang
            },
            new Citation
            {
                Title = "Crafting Interpreters",
                Authors = new List<string> { "Robert Nystrom" },
                Type = CitationType.Book,
                Year = 2021,
                Publisher = "Genever Benning",
                Isbn = "978-0990582939",
                Url = "https://craftinginterpreters.com/",
                Abstract = "Practical guide to building programming language interpreters, implementing two complete interpreters.",
                Tags = new List<string> { "interpreters", "compilers", "hands-on", "Lox" },
                DomainId = progLang
            },

            // ==================== TESTING & QUALITY ====================
            new Citation
            {
                Title = "Growing Object-Oriented Software, Guided by Tests",
                Authors = new List<string> { "Steve Freeman", "Nat Pryce" },
                Type = CitationType.Book,
                Year = 2009,
                Publisher = "Addison-Wesley",
                Isbn = "978-0321503626",
                Url = "http://www.growing-object-oriented-software.com/",
                Abstract = "TDD approach to building object-oriented systems with emphasis on mock objects and outside-in development.",
                Tags = new List<string> { "TDD", "mocks", "OOP", "seminal" },
                DomainId = testing
            },
            new Citation
            {
                Title = "xUnit Test Patterns: Refactoring Test Code",
                Authors = new List<string> { "Gerard Meszaros" },
                Type = CitationType.Book,
                Year = 2007,
                Publisher = "Addison-Wesley",
                Isbn = "978-0131495050",
                Url = "http://xunitpatterns.com/",
                Abstract = "Comprehensive catalog of test patterns and smells for writing maintainable automated tests.",
                Tags = new List<string> { "testing", "patterns", "xUnit", "refactoring" },
                DomainId = testing
            },
            new Citation
            {
                Title = "The Art of Software Testing",
                Authors = new List<string> { "Glenford Myers", "Corey Sandler", "Tom Badgett" },
                Type = CitationType.Book,
                Year = 2011,
                Publisher = "Wiley",
                Isbn = "978-1118031964",
                Url = "https://www.wiley.com/en-us/The+Art+of+Software+Testing%2C+3rd+Edition-p-9781118133156",
                Abstract = "Classic book on software testing principles and techniques covering black-box and white-box testing.",
                Tags = new List<string> { "testing", "principles", "classic", "techniques" },
                DomainId = testing
            },
            new Citation
            {
                Title = "Continuous Testing for DevOps Professionals",
                Authors = new List<string> { "Eran Kinsbruner" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "Independently Published",
                Isbn = "978-1720008056",
                Url = "https://www.perfecto.io/resources/continuous-testing-devops",
                Abstract = "Guide to implementing continuous testing in DevOps pipelines covering tools, strategies, and best practices.",
                Tags = new List<string> { "continuous-testing", "DevOps", "automation", "CI/CD" },
                DomainId = testing
            },
            new Citation
            {
                Title = "Lessons Learned in Software Testing",
                Authors = new List<string> { "Cem Kaner", "James Bach", "Bret Pettichord" },
                Type = CitationType.Book,
                Year = 2002,
                Publisher = "Wiley",
                Isbn = "978-0471081128",
                Url = "https://www.wiley.com/en-us/Lessons+Learned+in+Software+Testing%3A+A+Context+Driven+Approach-p-9780471081128",
                Abstract = "293 lessons on software testing from three experienced practitioners covering techniques, management, and careers.",
                Tags = new List<string> { "testing", "lessons", "context-driven", "experience" },
                DomainId = testing
            },
            new Citation
            {
                Title = "Property-Based Testing with PropEr, Erlang, and Elixir",
                Authors = new List<string> { "Fred Hebert" },
                Type = CitationType.Book,
                Year = 2019,
                Publisher = "Pragmatic Bookshelf",
                Isbn = "978-1680506211",
                Url = "https://pragprog.com/titles/fhproper/property-based-testing-with-proper-erlang-and-elixir/",
                Abstract = "Introduction to property-based testing using PropEr for finding edge cases automatically.",
                Tags = new List<string> { "property-testing", "Erlang", "Elixir", "automated" },
                DomainId = testing
            },

            // ==================== WEB DEVELOPMENT ====================
            new Citation
            {
                Title = "JavaScript: The Good Parts",
                Authors = new List<string> { "Douglas Crockford" },
                Type = CitationType.Book,
                Year = 2008,
                Publisher = "O'Reilly Media",
                Isbn = "978-0596517748",
                Url = "https://www.oreilly.com/library/view/javascript-the-good/9780596517748/",
                Abstract = "Guide to the subset of JavaScript that is truly useful, avoiding the language's problematic features.",
                Tags = new List<string> { "JavaScript", "best-practices", "classic" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "You Don't Know JS Yet",
                Authors = new List<string> { "Kyle Simpson" },
                Type = CitationType.Book,
                Year = 2020,
                Publisher = "Independently Published",
                Url = "https://github.com/getify/You-Dont-Know-JS",
                Abstract = "Deep dive into JavaScript mechanics covering scope, closures, types, and async programming.",
                Tags = new List<string> { "JavaScript", "deep-dive", "free", "comprehensive" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "React Documentation",
                Authors = new List<string> { "Meta" },
                Type = CitationType.Website,
                Year = 2024,
                Url = "https://react.dev/",
                Abstract = "Official React documentation covering components, hooks, and best practices for building UIs.",
                Tags = new List<string> { "React", "frontend", "documentation", "official" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "CSS: The Definitive Guide",
                Authors = new List<string> { "Eric A. Meyer", "Estelle Weyl" },
                Type = CitationType.Book,
                Year = 2017,
                Publisher = "O'Reilly Media",
                Isbn = "978-1449393199",
                Url = "https://www.oreilly.com/library/view/css-the-definitive/9781449325053/",
                Abstract = "Comprehensive reference to CSS covering selectors, layouts, animations, and modern features.",
                Tags = new List<string> { "CSS", "styling", "reference", "comprehensive" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "Web API Design: The Missing Link",
                Authors = new List<string> { "Apigee" },
                Type = CitationType.Website,
                Year = 2023,
                Url = "https://cloud.google.com/files/apigee/apigee-web-api-design-the-missing-link-ebook.pdf",
                Abstract = "Best practices for designing pragmatic RESTful APIs covering resource naming, versioning, and pagination.",
                Tags = new List<string> { "API", "REST", "design", "best-practices" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "The Node.js Design Patterns Bible",
                Authors = new List<string> { "Mario Casciaro", "Luciano Mammino" },
                Type = CitationType.Book,
                Year = 2024,
                Publisher = "Packt",
                Isbn = "978-1803243658",
                Url = "https://www.nodejsdesignpatterns.com/",
                Abstract = "Comprehensive guide to Node.js patterns covering asynchronous programming, streams, and scalability.",
                Tags = new List<string> { "Node.js", "patterns", "backend", "JavaScript" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "Eloquent JavaScript",
                Authors = new List<string> { "Marijn Haverbeke" },
                Type = CitationType.Book,
                Year = 2024,
                Publisher = "No Starch Press",
                Isbn = "978-1718504103",
                Url = "https://eloquentjavascript.net/",
                Abstract = "Modern introduction to JavaScript programming covering the browser, Node.js, and language fundamentals.",
                Tags = new List<string> { "JavaScript", "beginner-friendly", "free", "comprehensive" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "Progressive Web Apps",
                Authors = new List<string> { "Jason Grigsby" },
                Type = CitationType.Book,
                Year = 2018,
                Publisher = "A Book Apart",
                Isbn = "978-1937557720",
                Url = "https://abookapart.com/products/progressive-web-apps",
                Abstract = "Guide to building progressive web apps that work offline and provide native-like experiences.",
                Tags = new List<string> { "PWA", "offline", "service-workers", "mobile" },
                DomainId = webDev
            },

            // ==================== MICROSOFT .NET ECOSYSTEM ====================
            new Citation
            {
                Title = "ASP.NET Core Security and Identity",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/aspnet/core/security/",
                Abstract = "Comprehensive guide to authentication, authorization, data protection, and security best practices in ASP.NET Core applications including Identity, OAuth, OpenID Connect, and policy-based authorization.",
                Tags = new List<string> { "ASP.NET Core", "security", "authentication", "authorization", "Identity", "Microsoft" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "ASP.NET Core Authentication with External Providers",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/",
                Abstract = "Documentation for integrating external authentication providers (Google, Facebook, Microsoft, Twitter) with ASP.NET Core Identity, including OAuth 2.0 and OpenID Connect configuration.",
                Tags = new List<string> { "ASP.NET Core", "OAuth", "OpenID Connect", "social-login", "Microsoft" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "Blazor Documentation",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/aspnet/core/blazor/",
                Abstract = "Official documentation for Blazor, Microsoft's framework for building interactive web UIs using C# instead of JavaScript. Covers Blazor Server, Blazor WebAssembly, components, data binding, and JavaScript interop.",
                Tags = new List<string> { "Blazor", "WebAssembly", "C#", ".NET", "SPA", "Microsoft" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "Blazor State Management",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/aspnet/core/blazor/state-management",
                Abstract = "Guide to managing state in Blazor applications including browser storage, in-memory state containers, and state persistence across circuits and prerendering scenarios.",
                Tags = new List<string> { "Blazor", "state-management", "cascading-parameters", "Microsoft" },
                DomainId = webDev
            },
            new Citation
            {
                Title = ".NET Documentation",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/dotnet/",
                Abstract = "Official documentation for .NET platform covering .NET Core, .NET 8/9, C#, F#, ASP.NET Core, and cross-platform development. Includes tutorials, API reference, and architectural guidance.",
                Tags = new List<string> { ".NET", "C#", "cross-platform", "Microsoft", "documentation" },
                DomainId = progLang
            },
            new Citation
            {
                Title = ".NET Fundamentals",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/dotnet/fundamentals/",
                Abstract = "Core concepts of .NET including the runtime, libraries, SDK, deployment models, and architectural decisions. Essential reading for understanding the .NET ecosystem.",
                Tags = new List<string> { ".NET", "runtime", "SDK", "fundamentals", "Microsoft" },
                DomainId = progLang
            },
            new Citation
            {
                Title = "Entity Framework Core Documentation",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/ef/core/",
                Abstract = "Official documentation for Entity Framework Core, the modern object-database mapper for .NET. Covers DbContext, migrations, querying, change tracking, relationships, and performance optimization.",
                Tags = new List<string> { "Entity Framework", "EF Core", "ORM", "database", ".NET", "Microsoft" },
                DomainId = databases
            },
            new Citation
            {
                Title = "EF Core Performance Best Practices",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/ef/core/performance/",
                Abstract = "Performance optimization guide for Entity Framework Core including efficient querying, tracking vs no-tracking, compiled queries, bulk operations, and database indexing strategies.",
                Tags = new List<string> { "EF Core", "performance", "optimization", "querying", "Microsoft" },
                DomainId = databases
            },
            new Citation
            {
                Title = "Microsoft Entra ID Documentation",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/entra/identity/",
                Abstract = "Documentation for Microsoft Entra ID (formerly Azure Active Directory) covering identity management, single sign-on, multi-factor authentication, conditional access, and enterprise application integration.",
                Tags = new List<string> { "Entra", "Azure AD", "identity", "SSO", "MFA", "Microsoft" },
                DomainId = security
            },
            new Citation
            {
                Title = "Microsoft Identity Platform Documentation",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/entra/identity-platform/",
                Abstract = "Developer documentation for the Microsoft identity platform including OAuth 2.0, OpenID Connect, MSAL libraries, token handling, and integrating authentication into web, mobile, and desktop applications.",
                Tags = new List<string> { "MSAL", "OAuth", "OpenID Connect", "tokens", "identity", "Microsoft" },
                DomainId = security
            },
            new Citation
            {
                Title = "Microsoft REST API Guidelines",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Standard,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://github.com/microsoft/api-guidelines",
                Abstract = "Microsoft's REST API design guidelines covering URL structure, versioning, error handling, pagination, filtering, and consistency patterns. Widely adopted industry standard for API design.",
                Tags = new List<string> { "REST", "API", "design-guidelines", "standards", "Microsoft" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Azure REST API Design Guidelines",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Standard,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/azure/architecture/best-practices/api-design",
                Abstract = "Best practices for designing RESTful web APIs on Azure including resource naming, HTTP methods, status codes, HATEOAS, versioning strategies, and async operations.",
                Tags = new List<string> { "Azure", "REST", "API", "architecture", "best-practices", "Microsoft" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Azure Architecture Center",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/azure/architecture/",
                Abstract = "Comprehensive resource for cloud and software architecture including reference architectures, design patterns, best practices, and architecture decision records for building reliable, scalable systems.",
                Tags = new List<string> { "Azure", "architecture", "patterns", "cloud", "reference", "Microsoft" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = ".NET Application Architecture Guidance",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/dotnet/architecture/",
                Abstract = "Architecture guidance for .NET applications including microservices, containerization, Domain-Driven Design, CQRS, event sourcing, and cloud-native application patterns.",
                Tags = new List<string> { ".NET", "architecture", "microservices", "DDD", "cloud-native", "Microsoft" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Unit Testing Best Practices in .NET",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices",
                Abstract = "Best practices for unit testing .NET applications including naming conventions, test organization, avoiding logic in tests, and guidelines for writing maintainable test suites.",
                Tags = new List<string> { "testing", "unit-tests", "best-practices", ".NET", "Microsoft" },
                DomainId = testing
            },
            new Citation
            {
                Title = ".NET Testing with xUnit",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test",
                Abstract = "Guide to unit testing .NET projects using xUnit test framework including test project setup, assertions, test fixtures, parameterized tests, and integration with dotnet test CLI.",
                Tags = new List<string> { "xUnit", "testing", ".NET", "TDD", "Microsoft" },
                DomainId = testing
            },
            new Citation
            {
                Title = "Integration Testing in ASP.NET Core",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests",
                Abstract = "Documentation for integration testing ASP.NET Core applications using WebApplicationFactory, TestServer, and strategies for testing middleware, filters, and the full request pipeline.",
                Tags = new List<string> { "integration-testing", "ASP.NET Core", "WebApplicationFactory", "Microsoft" },
                DomainId = testing
            },
            new Citation
            {
                Title = "Blazor Component Testing with bUnit",
                Authors = new List<string> { "Egil Hansen" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "bUnit",
                Url = "https://bunit.dev/",
                Abstract = "Testing library for Blazor components that enables writing comprehensive tests. Supports testing component rendering, events, two-way binding, dependency injection, and JavaScript interop.",
                Tags = new List<string> { "bUnit", "Blazor", "testing", "components", ".NET" },
                DomainId = testing
            },
            new Citation
            {
                Title = "C# Coding Conventions",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions",
                Abstract = "Official C# coding conventions covering naming, layout, commenting, and language usage guidelines. Essential for maintaining consistent, readable C# codebases.",
                Tags = new List<string> { "C#", "coding-style", "conventions", "best-practices", "Microsoft" },
                DomainId = progLang
            },
            new Citation
            {
                Title = "ASP.NET Core Middleware",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/",
                Abstract = "Documentation for ASP.NET Core middleware pipeline including built-in middleware, custom middleware development, ordering, branching, and request/response modification patterns.",
                Tags = new List<string> { "ASP.NET Core", "middleware", "pipeline", "HTTP", "Microsoft" },
                DomainId = webDev
            },
            new Citation
            {
                Title = "Dependency Injection in .NET",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection",
                Abstract = "Guide to dependency injection in .NET applications covering service lifetimes, registration patterns, IServiceCollection, and best practices for building loosely coupled applications.",
                Tags = new List<string> { ".NET", "dependency-injection", "IoC", "services", "Microsoft" },
                DomainId = softwareEng
            },
            new Citation
            {
                Title = "Configuration in ASP.NET Core",
                Authors = new List<string> { "Microsoft" },
                Type = CitationType.Manual,
                Year = 2024,
                Publisher = "Microsoft",
                Url = "https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/",
                Abstract = "Documentation for ASP.NET Core configuration system including options pattern, configuration providers, secrets management, and environment-specific configuration.",
                Tags = new List<string> { "ASP.NET Core", "configuration", "options-pattern", "secrets", "Microsoft" },
                DomainId = webDev
            },
        });

        // Set DateAdded for all citations
        var now = DateTime.UtcNow;
        foreach (var citation in citations)
        {
            citation.Id = Guid.NewGuid();
            citation.DateAdded = now.AddMinutes(-citations.IndexOf(citation)); // Stagger dates slightly
            citation.DateModified = citation.DateAdded;
            citation.Authors ??= new List<string>();
            citation.Tags ??= new List<string>();
        }

        return citations;
    }
}
