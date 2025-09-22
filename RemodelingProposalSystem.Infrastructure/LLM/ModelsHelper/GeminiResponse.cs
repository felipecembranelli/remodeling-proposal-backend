using System.Text.Json.Serialization;

namespace RemodelingProposalSystem.Infrastructure.LLM.Models
{
    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[] Candidates { get; set; }

        [JsonPropertyName("usageMetadata")]
        public UsageMetadata UsageMetadata { get; set; }

        [JsonPropertyName("modelVersion")]
        public string ModelVersion { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("avgLogprobs")]
        public double AvgLogprobs { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public Part[] Parts { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class UsageMetadata
    {
        [JsonPropertyName("promptTokenCount")]
        public int PromptTokenCount { get; set; }

        [JsonPropertyName("candidatesTokenCount")]
        public int CandidatesTokenCount { get; set; }

        [JsonPropertyName("totalTokenCount")]
        public int TotalTokenCount { get; set; }

        [JsonPropertyName("promptTokensDetails")]
        public TokenDetail[] PromptTokensDetails { get; set; }

        [JsonPropertyName("candidatesTokensDetails")]
        public TokenDetail[] CandidatesTokensDetails { get; set; }
    }

    public class TokenDetail
    {
        [JsonPropertyName("modality")]
        public string Modality { get; set; }

        [JsonPropertyName("tokenCount")]
        public int TokenCount { get; set; }
    }
} 