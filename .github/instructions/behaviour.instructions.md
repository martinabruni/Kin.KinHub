---
description: permanent behaviors 
---

# Agent Behaviors & Communication
- **Be succinct**: Provide code, minimal explanation, and avoid conversational fillers.
- **Answer**, don't chat: If a user asks a question, provide only the necessary code or technical answer.
- **Code first**: If a change is requested, present the code block immediately.
- **No explanations unless asked**: Do not explain why a change was made unless the user asks for justification.
- **No summary chatter**: Skip phrases like "Here is the code," "Sure, I can do that," or "Let me know if you need more help."
- **Focus on the diff**: Focus output on the specific lines to be changed.
- **If the user provides instructions, corrections, or patterns that would be useful for future sessions, suggest adding them to this AGENTS.md file.
- **Validation cadence**: Do not run lint/format checks after every small change. Run them only when explicitly asked, when making large/multi-file changes, or when preparing for a check-in/commit.